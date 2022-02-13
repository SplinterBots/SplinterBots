namespace Functional.SplinterBots.Storage

module Types =

    open System
    open LiteDB

    type ChestType =
        | Daily
        | Season

    type RewardStatistics = 
        {
            id: int
            reportedOn: DateTime
            user: string
            chestType: ChestType
            rewards: string seq
        }
    module RewardStatistics =
        let bind (reportedOn: DateTime) user chestType rewards = 
            let id = $"{user}{reportedOn}{chestType}".GetHashCode()
            {
                id = id
                reportedOn = reportedOn
                user = user
                chestType = chestType
                rewards = rewards 
            }

    type Currency = 
        | DEC
        | SPS

    type TransferType =
        | Donation
        | MainAccount

    type Transfers = 
        {
            id: int
            reportedOn: DateTime
            user: string
            transferType: TransferType
            currency: Currency
            amount: decimal 
        }
    module Transfers =
        let bind currency user transferType amount =
            let reportedOn = System.DateTime.Now.Date
            let idDate = reportedOn.ToString()
            let id = $"{user}{idDate}{transferType}{currency}".GetHashCode()
            {
                id = id
                reportedOn = reportedOn
                user = user
                transferType = transferType
                currency = currency
                amount = amount
            }

        let bindDec =
            bind Currency.DEC

        let bindSps =
            bind Currency.SPS
