namespace Functional.SplinterBots

module DEC =
    open Defaults
    open SplinterBotsMath
    open API.Transfer
    open Functional.SplinterBots.API.Player

    let donateDec (context: ExecutionContext.Context) =
        async {
            let playerBalance = context.playerBalance

            let decToTransfer = calculateDecDonation playerBalance.dec

            //if decToTransfer.IsSome
            //then
            match decToTransfer with
            | Some transferDec ->
                do! transferCurrency Token.DEC transferDec donateAccountName context.playerName context.activeKey
                return {
                    context with
                        lastTransferValue = decToTransfer.Value;
                        playerBalance = { context.playerBalance with dec = context.playerBalance.dec - transferDec }
                }
            | None ->
                return context
        }
    let transferDecToMainAccount limit (context: ExecutionContext.Context) =
        async {
            let playerBalance = context.playerBalance

            let decToTransfer = playerBalance.dec - limit

            if decToTransfer > 0.001M
            then
                do! transferCurrency Token.DEC decToTransfer context.destinationAccount context.playerName context.activeKey
                return {
                    context with
                        lastTransferValue = decToTransfer;
                        playerBalance = { context.playerBalance with dec = context.playerBalance.dec - decToTransfer }
                }
            else
                return context
        }
