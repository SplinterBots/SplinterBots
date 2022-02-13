module SplinterlandToken

open Functional.SplinterBots.Storage

type Token = 
    {
        id: string
        token: string 
    }
module Token =
    let bind user token =
        {
            id = user
            token = token
        }

module Queries = 
    let getToken (store: Store.Store<Token>) playerName = 
        let token = store.GetById playerName
        match token with 
        | Patterns.NotNull -> 
            token.token
        | _ -> 
            ""
