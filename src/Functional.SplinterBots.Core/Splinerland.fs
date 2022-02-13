namespace Functional.SplinterBots

module Splinterland =
    
    open Functional.SplinterBots.API
    
    let settings (context: ExecutionContext.Context) =
        async {
            let! settings = Settings.getSettings ()
            return { context with settings = settings }
        }

    let updateToken (context: ExecutionContext.Context) = 
        async {
            let savedToken = context.authToken
            let! isTokenStillValid = API.Login.valiadteToken context.playerName savedToken

            match isTokenStillValid with
            | true -> 
                return { context with authToken = savedToken }
            | _ -> 
                let! newToken = API.Login.getToken context.playerName context.postingKey
                return { context with authToken = newToken }             
        }
