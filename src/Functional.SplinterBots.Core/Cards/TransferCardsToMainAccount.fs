namespace Functional.SplinterBots.Cards

module TransferCardsToMainAccount =
    open Cards
    open Functional.SplinterBots.API
    open Functional.SplinterBots.ExecutionContext

    let sentCardsToMainAccount (context: Context) =
        async {
            let playerName = context.playerName
            let! playerCollection = Cards.getAvailableCardsForPlayer playerName
            let playerCollection  = playerCollection |> Cards.filterCardsByOwningPlayer playerName
            if( Seq.length playerCollection > 0)
            then 
                let cardsToTransfer = 
                    playerCollection
                    |> Seq.map (fun cards -> cards.uid)
                    |> IdConverter.bindMultipleCardsId
            
                Cards.sentCards cardsToTransfer context.destinationAccount playerName context.activeKey 
                |> ignore

            return context
        }   
