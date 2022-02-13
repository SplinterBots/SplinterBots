namespace Functional.SplinterBots

module SPS =

    open Defaults
    open SplinterBotsMath
    open API.Transfer

    let claimSps (context: ExecutionContext.Context) =
         async {
            let (ts, signature) = API.Signature.sign $"hive{context.playerName}" context.postingKey
            do! API.Transfer.claimSPS context.playerName ts signature context.authToken

            return context
        }

    let donateSps (context: ExecutionContext.Context) =
        async {
            let details = context.playerBalance

            let spsToTransfer =  calcualteSPSDonation details.sps

            match spsToTransfer with
            | Some transferSps ->
                do! transferCurrency Token.SPS transferSps donateAccountName context.playerName context.activeKey
                return {
                    context with
                        lastTransferValue = spsToTransfer.Value;
                        playerBalance = { context.playerBalance with sps = context.playerBalance.sps - transferSps }
                }
            | None ->
                return context
        }
    let transferSPSToMainAccount (context: ExecutionContext.Context) =
        async {
            let details = context.playerBalance

            let spsToTransfer = details.sps

            if spsToTransfer > 0.001M
            then
                do! transferCurrency Token.SPS spsToTransfer context.destinationAccount context.playerName context.activeKey
                return {
                    context with
                        lastTransferValue = spsToTransfer;
                        playerBalance = { context.playerBalance with sps = context.playerBalance.sps - spsToTransfer }
                }
            else
                return context
        }
