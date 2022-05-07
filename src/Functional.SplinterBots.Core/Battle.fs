namespace Functional.SplinterBots

open Functional.SplinterBots.API
open Functional.SplinterBots.BattleAPI.Battler
open Functional.SplinterBots.BattleAPI.WebSocket
open ExecutionContext
open Newtonsoft.Json.Linq
open System.Threading.Tasks
open Ultimate_Splinterlands_Bot_V2.Api
open Functional.SplinterBots.BattleAPI

module Battle =

    let getTeam cards username  (matchDeatils: MatchDetails) = 
        async {
            let! team = 
                UltimateTeam.GetTeamFromPublicAPIAsync
                    matchdetails.mana_cap
                    matchdetails.rulesets
                    matchdetails.allowedSplinters
                    cards
                    matchdetails.
                    matchdetails.
                    matchdetails.
                    matchdetails.
                    matchdetails.
        }
    let playBattle(context: Context) =
        async {
            let username = context.playerName
            let postingKey = context.postingKey
            let accessToken = context.authToken


            use socket = new WebSocketListener ("wss://ws2.splinterlands.com/", username, accessToken)
            do! fight startNewMatch getTeam submitTeam revealTeam socket username postingKey 
            return context
        }
