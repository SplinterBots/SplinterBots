namespace Functional.SplinterBots

[<AutoOpen>]
module Status =
    open System
    open API.Quest

    type Username = string
    type RetryCount = int
    type CardId = string
    type TimeToEnd = string
    type Token = string
    type SeasonNumber = int
    type SeasonReward =
        | InProgress of TimeToEnd
        | ReadyToClaim
        | Claimed
        | Unknown

    type SplinterlandLeague =
        | Novice = 0
        | Bronze_III = 1
        | Bronze_II = 2
        | Bronze_I = 3
        | Silver_III = 4
        | Silver_II = 5
        | Silver_I = 6
        | Gold_III = 7
        | Gold_II = 8
        | Gold_I = 9
        | Diamond_III = 10
        | Diamond_II = 11
        | Diamond_I = 12
        | Champion_III = 13
        | Champion_II = 14
        | Champion_I = 15

    module SplinterlandLeague =
        let leaugeToPower leauge =
            match leauge with 
            | Novice -> 1000
            | Bronze_III -> 1000
            | Bronze_II -> 1000
            | Bronze_I -> 1000
            | _ -> 1000

    type AccountDetail =
        {
            username: string
            dec: decimal
            sps: decimal
            credits: decimal
            rating: int
            power: int
            sesonalReward: SeasonReward
            dailyQuestName: string
            dailyReward: DailyReward
            leauge: SplinterlandLeague
            ecr: decimal
        }
    module AccountDetail =
        let notLoaded =
            {
                username = ""
                dec = 0.0M
                sps = 0.0M
                credits = 0.0M
                rating = 0
                power = 0
                ecr = 00M
                sesonalReward = SeasonReward.Unknown
                dailyQuestName = ""
                dailyReward = DailyReward.Unknown
                leauge = SplinterlandLeague.Novice
            }
        let isNotLoaded detail =
            detail.dailyQuestName = "not loaded"
        let loading =
            {
                username = ""
                dec = 0.0M
                sps = 0.0M
                credits = 0.0M
                rating = 0
                power = 0
                ecr = 00M
                sesonalReward = SeasonReward.Unknown
                dailyQuestName = "loading ..."
                dailyReward = DailyReward.Unknown
                leauge = SplinterlandLeague.Novice
            }
        let bindLoading username =
            {loading with username = username }
    type ExecutionModes =
        | Claim
        | CheckDetails

    type SplinterlandStatus =
        | Ignore
        | Success
        | Error of exn

        | BrowserInitialisation
        | SetUpBrowser
        | CloseBrowser
        | ClosePage

        | ClosePopup

        | UpdateToken of Token

        | DonateDec
        | DecDonated of decimal
        | DecTransfered of decimal

        | ClaimSPS
        | DonateSps
        | SpsDonated of decimal
        | SpsTransfered of decimal

        | ClaimDailyReward
        | ClaimedDailyReward of DateTime * string seq
        | ClaimSeasonReward
        | ClaimedSeasonReward of DateTime * string seq

        | CardTransfered of CardId

        | StartedProcessing of Username
        | Mode of ExecutionModes

        | LoadedAccountDetails of AccountDetail

        | RentingPreselectedCards
        | RentingCardsToFillPower
        | CardsRented

        | TransferCardsToMainAccount

        | BuyCardWithCredtis

        | Processed of Username
        | FinishedProcessing

        | ReTry of RetryCount * SplinterlandStatus

        | AllowToFail of SplinterlandStatus
