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

                //report ClaimDailyReward
                //Actions.ensureOperationIsAllowed
                //    config.claimDailyReward
                //    Rewards.claimDailyChest
                //        >>~ (Messages.reportDailyChestRewards log)

                //report ClaimSeasonReward
                //Actions.ensureOperationIsAllowed
                //    config.claimSeasonReward
                //    Rewards.claimSeasonChests
                //        >>~ (Messages.reportSeasonChestRewards log)

                //Actions.ensureOperationIsAllowed
                //    config.rentCards
                //    (Cards.RentSelectedCards.rentCardsSelectedByUser config.cards)
                //        >>~ (report RentingCardsToFillPower)
                //        >>~ (report RentingCardsToFillPower)
                //        >>~ (Cards.RentCardsWithDec.rentCardsToReachPower (config.desiredLeague |> SplinterlandLeague.leaugeToPower))
                //        >>~ (report CardsRented)
                
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

                //report DonateDec
                //DEC.donateDec
                //Messages.reportLastTransfer log DecDonated
                //DEC.transferDecToMainAccount config.decLimit
                //Messages.reportLastTransfer log DecTransfered

                //report BuyCardWithCredtis
                //Cards.BuyForCredits.buyCheapestCardOnMarketWithCredits

                Actions.ensureOperationIsAllowed
                    config.transferCardsToMainAccount
                    (report TransferCardsToMainAccount)
                        >>~ (Cards.TransferCardsToMainAccount.sentCardsToMainAccount)

                //report ClaimSPS
                //SPS.claimSps
                //AccountDetails.getAccountDetails
                //report DonateSps
                //SPS.donateSps
                //Messages.reportLastTransfer log SpsDonated
                //SPS.transferSPSToMainAccount
                //Messages.reportLastTransfer log SpsTransfered

                Splinterland.settings
                AccountDetails.getAccountDetails
                Messages.reportAccountDetailas log
                Messages.reportFinishProcessing log
            ]
        Actions.executeActions log getToken config getAccountDetail
