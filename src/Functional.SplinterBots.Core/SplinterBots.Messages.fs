namespace Functional.SplinterBots

module Messages = 
    
    open ExecutionContext
    
    let message log generateMessage context =
        async {
            do! log (async.Return (context.playerName , generateMessage context))
            return context 
        }
    let report log state =
        message log (fun c -> state)
    let reportLastTransfer log state context =
        message log (fun c -> state context.lastTransferValue) context
    let reportAccountDetailas log =
        message log (fun c -> LoadedAccountDetails (AccountDetails.buildAccountDetails c))
    let reportStartProcessing log =
        message log (fun c -> StartedProcessing c.playerName)
    let reportFinishProcessing log =
        message log (fun c -> Processed c.playerName)
    let reportDailyChestRewards log context =
        async {
            let! (claimDate, rewards) = Rewards.getClaimDailyRewards context.playerName
            
            match claimDate with
            | Some claimDate  -> 
                let! _ =  message log (fun c -> ClaimedDailyReward (claimDate, rewards)) context
                ()
            | None -> 
                ()

            return context
        }
    let reportSeasonChestRewards log context =
        async {
            let rewards = 
                let lastSeasonRewardsInfo = context.lastSeasonRewards
                lastSeasonRewardsInfo.rewards |> Seq.map API.Rewards.toString
            let date = context.settings.season.ends.AddDays(-15.0)
            let! _ = message log (fun c -> ClaimedSeasonReward (date, rewards)) context

            return context
        }
    let reportSplinterlandToken log  = 
        message log (fun c -> UpdateToken c.authToken)
