namespace Functional.SplinterBots

open Functional.SplinterBots.API
open Functional.SplinterBots.BattleAPI.Battler
open Functional.SplinterBots.BattleAPI.WebSocket
open ExecutionContext
open Newtonsoft.Json.Linq
open System.Threading.Tasks
open Ultimate_Splinterlands_Bot_V2.Api
open Functional.SplinterBots.BattleAPI
open Ultimate_Splinterlands_Bot_V2.Bot
open Functional.SplinterBots.BattleAPI.WebSocket2
open FsHttp
open FsHttp.DslCE
open FsHttp.Response
open System.Text.Json
open System
open System.Threading

module Battle =

    let executeApiPostCall2<'T> url payload = 
        async {
            let! response =
                httpAsync {                   
                    POST url
                    UserAgent "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:93.0) Gecko/20100101 Firefox/93.0"
                    ContentType "application/x-www-form-urlencoded"                                      
                    body 
                    json payload
                }
        
            let test = response |> toString Int32.MaxValue
            let! responseStream =  response |> toStreamAsync
            return! JsonSerializer.DeserializeAsync<'T>(responseStream).AsTask() |> Async.AwaitTask 
        }
    
    let mutable jTeam = JToken.Parse("")
    let fight2 
           (startFight: StartFight) 
           (getTeam: GetTeam) 
           (submitTeam: SubmitTeam) 
           (revealTeam: RevealTeam)
           (webSocket: WebSocketListener2)
           (username: Username)
           (postingKey: PostingKey) =
       async {
           let! transaction = startFight username postingKey
           let transactionFound = webSocket.WaitForTransaction transaction.id
           let matchFound = webSocket.WaitForGamesState GameState.match_found 

           Thread.Sleep (2000)

           let matchDetails = webSocket.GetState GameState.match_found |> MatchDetails.bind
           let! team = getTeam matchDetails

           do! (Task.Delay 8000 |> Async.AwaitTask)

           let! submitedTeam = submitTeam transaction team
           let _ = webSocket.WaitForTransaction submitedTeam.id

           let! revealedTeam = revealTeam transaction team
           ()
       }

    let startNewMatch2 username postingkey =
       async {
           let n = Generator.generateRandomString 10
           let json = sprintf "{\"match_type\":\"Ranked\",\"app\":\"%s\",\"n\":\"%s\"}"

           let customJson = API.createCustomJsonPostingKey username "sm_find_match" json
           let transaction = API.hive.create_transaction ([| customJson |], [| postingkey |])
           let postData = Generator.getStringForSplinterlandsAPI transaction
           let battleUrl = "https://battle.splinterlands.com/battle/battle_tx"
           let! response = executeApiPostCall2<Transaction> battleUrl postData
           return response
       }
       
    let getTeam username cards (matchdetails: MatchDetails) = 
        async {
            let! (quest, questDetails) = SplinterlandsAPI.GetPlayerQuestAsync(username) |> Async.AwaitTask
            let! teamResponse = 
                UltimateTeam.GetTeamFromPublicAPIAsync(
                    matchdetails.mana_cap,
                    matchdetails.rulesets,
                    matchdetails.allowedSplinters,
                    cards,
                    quest,
                    questDetails,
                    username) |> Async.AwaitTask

            let findCard cardId =  cards |> Seq.find (fun c -> c.card_detail_id = cardId)
            let summonerCard = teamResponse.["summoner_id"].ToObject<Int32> () |> findCard
            let monsters = 
                ["monster_1_id"; "monster_2_id"; "monster_3_id"; "monster_4_id"; "monster_5_id"; "monster_6_id"]
                |> Seq.map (fun monster -> teamResponse.[monster].ToObject<Int32> ())
                |> Seq.filter (fun card -> card > 0)
                |> Seq.map findCard

            return Team(summonerCard, monsters)
        }
    let teamHash (team: Team) =
        let monstersString = 
            team.Team
            |> Seq.map (fun monster -> monster.card_long_id)
            |> String.concat ","
        let toHash = 
            sprintf "%s,%s,%s"
                team.Summoner.card_long_id
                monstersString
                team.Secret
        generateMD5Hash toHash
    let submitTeam (botinstance: UltimateBot) (transaction: Transaction) (team: Team) = 
        async {
            //let hash1 = botinstance.TeamHash team
            let hash2 = teamHash team
            let! (_, transaction, _) = botinstance.SubmitTeamAsync(transaction.id, team) |> Async.AwaitTask
            return 
                {
                    id = "test"
                    success = true
                }
        }
    let revealTeam (botinstance: UltimateBot) (transaction: Transaction) (team: Team) = 
        async {
            botinstance.RevealTeam(transaction.id, team)
            let transaction = 
                {
                    id = "test"
                    success = true
                }
            return transaction
        }

    let playBattle(context: Context) =
        async {
            let username = context.playerName
            let postingKey = context.postingKey
            let accessToken = context.authToken


            use socket = new WebSocketListener2 ("wss://ws2.splinterlands.com/", username, accessToken)
            do! socket.Start ()
            let! cards = SplinterlandsAPI.GetPlayerCardsAsync(username) |> Async.AwaitTask
            let bot = new UltimateBot(username, postingKey, cards)
            do! fight2 startNewMatch2 (getTeam username cards) (submitTeam bot) (revealTeam bot) socket username postingKey 
            return context
        }
