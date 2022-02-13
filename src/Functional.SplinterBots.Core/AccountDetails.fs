namespace Functional.SplinterBots

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
            
            return 
                { 
                    context with 
                        playerBalance = playerBalance
                        playerDetails = playerDetails
                        playerQuest = getSome Quest.PlayerQuests.empty playerQuest 
                        lastSeasonRewards = lastSeasonRewards
                }
        }
