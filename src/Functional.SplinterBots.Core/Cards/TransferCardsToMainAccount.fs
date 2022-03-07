namespace Functional.SplinterBots.Cards

module RentCardsWithDec =
    open Cards
    open Functional.SplinterBots.API
    open Functional.SplinterBots.API.Market
    open Functional.SplinterBots.ExecutionContext

    let private daysToRentCards = 7

    let legendaryCards = 
        getCards() 
        |> Async.RunSynchronously 
        |> Cards.filterCardsByRarity CardRarity.Legendary
        |> Seq.map (fun card -> card.id)

    let filterCardsGroupByRarity (cards: Async<CardGroup seq>) =
        async {
            let! cards = cards

            return 
                cards 
                |> Seq.filter (fun group -> Seq.contains group.card_detail_id legendaryCards)
        }

    let private rentCard playerName activeKey = 
        async {
            let! cards = 
                Market.getCardsGroupFromMarket MarketAction.Rent
                |> filterCardsGroupByRarity

            let cardGroup = cards |> Seq.head
            let! cardsToBuy = cardGroup |> Market.getCardsDetailsFromMarket MarketAction.Rent
            let cardsToBuy = cardsToBuy |> Seq.head 

            Market.rentCards (bindSingleId cardsToBuy.market_id) Currency.DEC daysToRentCards playerName activeKey
        }
    let rec private ensurePowerLimitIsReached desiredPowerLimit playerName activeKey = 
        async {
            let! details = Player.Details.getDetails playerName
            let power = details.collection_power

            if power <= desiredPowerLimit
            then
                do! rentCard playerName activeKey
                do! ensurePowerLimitIsReached desiredPowerLimit playerName activeKey
            
            return ()
        }

    let rentCardsToReachPower desiredPowerLimit (context: Context) =
         async {
            let dec = context.playerBalance.dec
            
            if( dec > 20M)
            then 
                do! ensurePowerLimitIsReached desiredPowerLimit context.playerName context.activeKey
            
            return context
         }
