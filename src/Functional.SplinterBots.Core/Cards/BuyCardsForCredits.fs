namespace Functional.SplinterBots.Cards

module BuyForCredits =
    open Functional.SplinterBots.API
    open Functional.SplinterBots.API.Market
    open Functional.SplinterBots.API.Player.Balance
    open Functional.SplinterBots.ExecutionContext

    let buyCheapestCardOnMarketWithCredits (context: Context) =
        async {
            let credits = context.playerBalance.credits

            if credits > 50M
            then
                let! cardsGroupsOnMarket = Market.getCardsGroupFromMarket MarketAction.Buy
                let cheaperGroup = cardsGroupsOnMarket |> Seq.tryHead

                match cheaperGroup with
                | Some group ->
                    let! cardsToBuy = Market.getCardsDetailsFromMarket MarketAction.Buy group
                    let firstCard = cardsToBuy |> Seq.tryHead

                    match firstCard with
                    | Some card ->
                        let playerName = context.playerName
                        let activekey = context.activeKey
                        Market.buyCard card Currency.CREDITS playerName activekey |> ignore
                    | None ->
                        ()
                | _ ->
                    ()

            return context
        }
