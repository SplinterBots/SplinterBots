namespace Functional.SplinterBots

open Newtonsoft.Json.Linq
open System

module AccountDetails =

    open ExecutionContext
    open Functional.SplinterBots.API
    open Functional.SplinterBots.API.Player
    open Functional.SplinterBots.API.Quest

    let buildAccountDetails (context: Context) = 
        {
            username = context.playerName
            dec = context.playerBalance.dec
            sps = context.playerBalance.sps
            credits = context.playerBalance.credits
            rating = context.playerDetails.rating
            power = context.playerDetails.collection_power
            sesonalReward = SeasonReward.Unknown
            dailyReward = getDailyReward context.playerQuest
            dailyQuestName = context.playerQuest.name
            ecr = context.playerBalance.ecr / 100M
            leauge = enum<SplinterlandLeague>(context.playerDetails.league)
        }

    let getSome defaultValue opt = 
        match opt with 
        | Some x -> x
        | None -> defaultValue


    let getAccountDetails (context: Context) = 
        async {
            let username = context.playerName
            let! playerBalance =  Balance.getBalance username
            let! playerDetails =  Details.getDetails username
            let! playerQuest =  Quest.getQuest username
            let! lastSeasonRewards = LastSeasonRewards.getLastSeasonRewards username

            //let account = (API.hive.get_accounts username)
            //let elapsed = 
            //    let CURRENT_UNIX_TIMESTAMP = DateTimeOffset.Now.ToUnixTimeSeconds();
            //    let last_update_time = (account["voting_manabar"]["last_update_time"]).ToString() |> int64
            //    CURRENT_UNIX_TIMESTAMP - last_update_time
            //let totalShares = 
            //    let dumpVESTS input = 
            //        input.ToString().Replace("VESTS", "")
            //    let vesting_shares = account["vesting_shares"] |> dumpVESTS |> double
            //    let received_vesting_shares = account["received_vesting_shares"] |> dumpVESTS |> double
            //    let delegated_vesting_shares = account["delegated_vesting_shares"] |> dumpVESTS |> double
            //    vesting_shares + received_vesting_shares - delegated_vesting_shares
            //let maxMana = totalShares * double(1000000)
            //let currentMana = 
            //    let current_mana = (account["voting_manabar"]["current_mana"]) |> double
            //    let elapsedMana = elapsed * int64(maxMana)
            //    let elapsedManaByDay = elapsedMana / 43200L
            //    current_mana + (elapsedManaByDay |> double)
            //let rc = currentMana * double(100) / maxMana 

            return 
                { 
                    context with 
                        playerBalance = playerBalance
                        playerDetails = playerDetails
                        playerQuest = getSome Quest.PlayerQuests.empty playerQuest 
                        lastSeasonRewards = lastSeasonRewards
                }
        }
