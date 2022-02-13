namespace Functional.SplinterBots.Storage

module WatchMessages =

    open Types
    open Errors
    open Functional.SplinterBots.Status

    let saveMessagesToDatabase status  =
        async {
            let! (user, status) = status
               
            match status with
            | ClaimedDailyReward (date, rewards) when Seq.empty <> rewards -> 
                let rewardReport = RewardStatistics.bind date user Daily rewards 
                Stores.rewardsStore.Upsert rewardReport
                 
            | ClaimedSeasonReward (date, rewards) when Seq.empty <> rewards -> 
                let rewardReport = RewardStatistics.bind date user Season rewards 
                Stores.rewardsStore.Upsert rewardReport

            | DecDonated amount when amount > 0M ->
                let transfer = Transfers.bindDec user TransferType.Donation amount
                Stores.transfersStore.Upsert transfer

            | DecTransfered amount when amount > 0M ->
                let transfer = Transfers.bindDec user TransferType.MainAccount amount
                Stores.transfersStore.Upsert transfer

            | SpsDonated amount when amount > 0M ->
                let transfer = Transfers.bindSps user TransferType.Donation amount
                Stores.transfersStore.Upsert transfer

            | SpsTransfered amount when amount > 0M ->
                let transfer = Transfers.bindSps user TransferType.MainAccount amount
                Stores.transfersStore.Upsert transfer

            | UpdateToken token ->
                let token = SplinterlandToken.Token.bind user token
                Stores.tokensStore.Upsert token

            | Error exn -> 
                let error = Error.bind user exn 
                Stores.errorsStore.Upsert error

            | _ -> ()
        }
