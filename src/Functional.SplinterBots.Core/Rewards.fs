namespace Functional.SplinterBots

module Rewards =

    open ExecutionContext
    open API.Quest
    open API.LastSeasonRewards
 
    let claimDailyChest (context: Context) = 
        async {

            let details = getDailyReward context.playerQuest
            match details with 
            | DailyReward.ReadyToClaim -> 
                let! quest = API.Quest.getQuest context.playerName
                if quest.IsSome 
                then 
                    do! claimQuest quest.Value.id context.playerName context.postingKey
                    do! startNewQuest context.playerName context.postingKey
            | _ -> 
                ()
            return context 
        }

    let getClaimDailyRewards playerName = 
        async {
            let! questInfo = API.Quest.getQuest playerName
            match questInfo with 
            | Some quest when quest.claim_date.IsSome -> 
                let dailRewards = API.Quest.getClaimedDailyRewards quest
                return (quest.claim_date,  dailRewards)
            | _ -> 
                return (None, Seq.empty)
        }

    let getPreviousSeasonId context = 
         context.settings.season.id - 1 

    let claimSeasonChests (context: Context) = 
        async {
            let! lastSeasonReward = API.LastSeasonRewards.getLastSeasonRewards context.playerName
            
            if not lastSeasonReward.success 
            then 
                let seasonToClaim = getPreviousSeasonId context
                do! claimSeason seasonToClaim context.playerName context.postingKey
            
            return context 
        }
