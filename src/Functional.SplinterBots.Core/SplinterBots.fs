namespace Functional.SplinterBots

module SplinterBots =

    open Config
    open Actions

    let getAccountDetails log getToken config =
        let getAccountDetail  =
            [
                Messages.reportStartProcessing log

                Splinterland.updateToken
                Messages.reportSplinterlandToken log

                Splinterland.settings
                AccountDetails.getAccountDetails

                Messages.report log ClaimDailyReward
                Actions.ensureOperationIsAllowed
                    config.claimDailyReward
                    Rewards.claimDailyChest
                        >>~ (Messages.reportDailyChestRewards log)

                Messages.report log ClaimSeasonReward
                Actions.ensureOperationIsAllowed
                    config.claimSeasonReward
                    Rewards.claimSeasonChests
                        >>~ (Messages.reportSeasonChestRewards log)

                Actions.ensureOperationIsAllowed
                    config.rentCards
                    (Cards.RentSelectedCards.rentCardsSelectedByUser config.cards)
                        >>~ (Messages.report log RentingCardsToFillPower)
                        >>~ (Messages.report log RentingCardsToFillPower)
                        >>~ (Cards.RentCardsWithDec.rentCardsToReachPower (config.desiredLeague |> SplinterlandLeague.leaugeToPower))
                        >>~ (Messages.report log CardsRented)
                
                Splinterland.settings
                AccountDetails.getAccountDetails
                Messages.reportAccountDetailas log
                Messages.reportFinishProcessing log
            ]
        Actions.executeActions log getToken config getAccountDetail

    let claimDaily log getToken config =
        let getAccountDetail  =
            [
                Messages.reportStartProcessing log

                Splinterland.updateToken
                Messages.reportSplinterlandToken log

                Splinterland.settings
                AccountDetails.getAccountDetails

                Messages.report log DonateDec
                DEC.donateDec
                Messages.reportLastTransfer log DecDonated
                DEC.transferDecToMainAccount config.decLimit
                Messages.reportLastTransfer log DecTransfered

                Messages.report log BuyCardWithCredtis
                Cards.BuyForCredits.buyCheapestCardOnMarketWithCredits

                Messages.report log ClaimSPS
                SPS.claimSps
                AccountDetails.getAccountDetails
                Messages.report log DonateSps
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
