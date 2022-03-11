namespace Functional.SplinterBots.Cards

module RentSelectedCards =
    open Functional.SplinterBots.API
    open Functional.SplinterBots.API.Market
    open Functional.SplinterBots.API.Player.Balance
    open Functional.SplinterBots.ExecutionContext

    [<Literal>]
    let private daysToRentCards = 7

    let private convertCardNameToCardIds selectedCardsNames =
        async {
            let! cards = Cards.getCards()

            let cards =
                cards
                |> Seq.filter (fun card -> selectedCardsNames |> Array.contains card.name)
                |> Seq.map (fun card -> card.id)

            return cards
        }
    let private getCardGroupsFromMarket (cardId: Async<int seq>) =
        async {
            let! group = Market.getCardsGroupFromMarket MarketAction.Rent
            let! cards = cardId

            return
                group
                |> Seq.filter (fun group -> Seq.contains group.card_detail_id cards )
        }
    let private getAvailableCards (cardGroup: Async<CardGroup seq>) =
        async  {
            let! cardGroup = cardGroup
            return
                cardGroup
                |> Seq.map (Market.getCardsDetailsFromMarket MarketAction.Rent)
                |> Seq.map (fun asyncObject -> asyncObject |> Async.RunSynchronously)
                |> Seq.filter (fun cards ->  Seq.length cards > 0)
                |> Seq.map (fun cards -> Seq.item 0 cards)
                |> Seq.map (fun card -> card.market_id)
        }

    let rentCardsSelectedByUser (cardNames: string seq) (context: Context) =
        async {
            match cardNames |> Seq.isEmpty with
            | true  ->
                ()
            | _ ->
                let! cards =
                    convertCardNameToCardIds (cardNames |> Array.ofSeq)
                    |> getCardGroupsFromMarket
                    |> getAvailableCards
                Market.rentCards (IdConverter.bindMultipleCardsId cards) Currency.DEC daysToRentCards context.playerName context.activeKey |> ignore

            return context
        }
