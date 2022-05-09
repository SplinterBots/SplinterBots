namespace Functional.SplinterBots

open System
open Functional.SplinterBots.API
open Functional.SplinterBots.BattleAPI
open Functional.SplinterBots.BattleAPI.Battler
open Functional.SplinterBots.BattleAPI.WebSocket
open Ultimate_Splinterlands_Bot_V2.Api
open ExecutionContext

module Battle =
    let getUltimateTeam username (matchdetails: MatchDetails) = 
        async {
            let! cards = Cards.getPlayerCards username
            let cards = cards |> Array.ofSeq
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

    let playBattle(context: Context) =
        async {
            let username = context.playerName
            let postingKey = context.postingKey
            let accessToken = context.authToken

            use socket = new WebSocketListener ("wss://ws2.splinterlands.com/", username, accessToken)
            do! socket.Start ()
            do! fight username postingKey socket (getTeam username) 
            return context
        }
