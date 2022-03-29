namespace Functional.SplinterBots

module SplinterBots =

    open Config
    open Actions

    let getAccountDetails log getToken config =
        let report = Messages.report log
        let getAccountDetail  =
            [
                Messages.reportStartProcessing log

                Splinterland.updateToken
                Messages.reportSplinterlandToken log

                Splinterland.settings
                AccountDetails.getAccountDetails

                report ClaimDailyReward
                Actions.ensureOperationIsAllowed
                    config.claimDailyReward
                    Rewards.claimDailyChest
                        >>~ (Messages.reportDailyChestRewards log)

                report ClaimSeasonReward
                Actions.ensureOperationIsAllowed
                    config.claimSeasonReward
                    Rewards.claimSeasonChests
                        >>~ (Messages.reportSeasonChestRewards log)

                Actions.ensureOperationIsAllowed
                    config.rentCards
                    (Cards.RentSelectedCards.rentCardsSelectedByUser config.cards)
                        >>~ (report RentingCardsToFillPower)
                        >>~ (report RentingCardsToFillPower)
                        >>~ (Cards.RentCardsWithDec.rentCardsToReachPower (config.desiredLeague |> SplinterlandLeague.leaugeToPower))
                        >>~ (report CardsRented)
                
                Actions.ensureOperationIsAllowed
                    config.transferCardsToMainAccount
                    (report TransferCardsToMainAccount)
                        >>~ (Cards.TransferCardsToMainAccount.sentCardsToMainAccount)
                
                Splinterland.settings
                AccountDetails.getAccountDetails
                Messages.reportAccountDetailas log
                Messages.reportFinishProcessing log
            ]
        Actions.executeActions log getToken config getAccountDetail

    let claimDaily log getToken config =
        let report = Messages.report log
        let getAccountDetail  =
            [
                Messages.reportStartProcessing log

                Splinterland.updateToken
                Messages.reportSplinterlandToken log

                Splinterland.settings
                AccountDetails.getAccountDetails

                report DonateDec
                DEC.donateDec
                Messages.reportLastTransfer log DecDonated
                DEC.transferDecToMainAccount config.decLimit
                Messages.reportLastTransfer log DecTransfered

                Actions.ensureOperationIsAllowed
                    config.buyCardWithCredits
                    Cards.BuyForCredits.buyCheapestCardOnMarketWithCredits
                        >>~ (report BuyCardWithCredtis)

                report ClaimSPS
                SPS.claimSps
                AccountDetails.getAccountDetails
                report DonateSps
                SPS.donateSps
                Messages.reportLastTransfer log SpsDonated
                SPS.transferSPSToMainAccount
                Messages.reportLastTransfer log SpsTransfered

                Splinterland.settings
                AccountDetails.getAccountDetails
                Messages.reportAccountDetailas log
                Messages.reportFinishProcessing log
            ]
        Actions.executeActions log getToken config getAccountDetail

    let playBattle log getToken config =
        let report = Messages.report log
        let getAccountDetail  =
            [
                Messages.reportStartProcessing log
                Splinterland.updateToken
                Messages.reportSplinterlandToken log
                Splinterland.settings
                AccountDetails.getAccountDetails

                Battle.playBattle

                Splinterland.settings
                AccountDetails.getAccountDetails
                Messages.reportAccountDetailas log
                Messages.reportFinishProcessing log
            ]
        Actions.executeActions log getToken config getAccountDetail
