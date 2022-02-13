namespace Functional.SplinterBots.Storage

module Stores =

    open Store
    open Types
    open Errors
    open SplinterlandToken

    let rewardsStore = 
        Store<RewardStatistics>("splintetrland")
    
    let transfersStore = 
        Store<Transfers>("splintetrland")

    let errorsStore = 
        Store<Error>("splintetrland")

    let tokensStore = 
        Store<Token>("splintetrland")

    let (|NotNull|_|) value = 
        if obj.ReferenceEquals(value, null) 
        then None 
        else Some()
