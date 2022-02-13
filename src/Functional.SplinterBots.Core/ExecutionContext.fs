namespace Functional.SplinterBots

module ExecutionContext = 
    open Config
    open Functional.SplinterBots.API.Player.Balance
    open Functional.SplinterBots.API.Player.Details
    open Functional.SplinterBots.API.Quest
    open Functional.SplinterBots.API.Settings    
    open Functional.SplinterBots.API.LastSeasonRewards

    type Context = 
        {
            playerName: string 
            postingKey: string
            activeKey: string 
            destinationAccount: string
            lastTransferValue: decimal
            playerBalance: PlayerBalance
            playerDetails: PlayerDetails
            playerQuest: PlayerQuests
            settings: Setting
            lastSeasonRewards: SeasonRewards
            authToken: string
        }
    let bindContext (transferDetails: TransferDetails) = 
        let context =
            {
                playerName =  transferDetails.username
                postingKey = transferDetails.postingKey
                activeKey = transferDetails.activeKey
                destinationAccount = transferDetails.destinationAccount
                lastTransferValue = 0M
                playerBalance = PlayerBalance.empty
                playerDetails = PlayerDetails.empty
                playerQuest = PlayerQuests.empty
                settings = Setting.empty
                lastSeasonRewards = SeasonRewards.empty
                authToken = ""
            }
        context

    type AsyncContext = Async<Context>
