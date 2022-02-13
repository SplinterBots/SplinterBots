namespace Functional.SplinterBots

module Actions = 
    
    open Config
    open ExecutionContext

    let ensureOperationIsAllowed check action =
        let inline doNothing context =
            async { return context }
        match check with 
        | true -> action
        | _ -> doNothing

    let rec private fold log (collection: list<Context -> Async<Context>>) (state:Context) =
        let tryAction log action state =
            let secureAction = action state
            let handleError (exn: exn) =
                async {
                    async.Return (state.playerName, Error exn) |> log |> Async.RunSynchronously
                    return state
                }
            async.TryWith (secureAction, handleError)
        async {
            match collection with 
            | [] ->
                return state
            | action :: tail -> 
                let! newState = tryAction log action state
                return! (fold log tail newState)
        }

    let executeActions log getToken config actions = 
        let degreeOfParallelism = config.parallelExecution

        async {
            let tasks = 
                config.transferDetails 
                |> Seq.map bindContext
                |> Seq.map (fun c -> { c with authToken = getToken c.playerName })
                |> Seq.map (fold log actions)

            let! _ = Async.Parallel(tasks, degreeOfParallelism)

            do! async.Return ("", FinishedProcessing) |> log
        }
   

    let (>>~) left right context =
        async {
            let! leftContext = left context
            let! rightContext =  right leftContext
    
            return rightContext
        }
