module BackgroundWorker

open System
open Functional.SplinterBots
open System.Reactive

type StatusObserver = IObserver<Async<string * SplinterlandStatus>>
type StatusList = Collections.Generic.List<StatusObserver>

type Unsubscriber (observers: StatusList, observer: StatusObserver) = 
    interface IDisposable with 
        member this.Dispose () =
            if observers.Contains(observer)
            then observers.Remove(observer) |> ignore

type BackgroundWorker () =
    let observers = new StatusList()
    let logToObserver status = 
        async {
            observers |> Seq.iter (fun observer -> observer.OnNext status)
        }
    let getToken = SplinterlandToken.Queries.getToken Storage.Stores.tokensStore
    let processor = 
        MailboxProcessor<ExecutionModes>.Start(fun inbox -> 
            async {
                while true do 
                    let! command = inbox.Receive()
                    let config = Config.getConfiguration ()
    
                    try 
                        match command with 
                        | ExecutionModes.CheckDetails -> 
                            do! SplinterBots.getAccountDetails logToObserver getToken config
                        | ExecutionModes.Claim -> 
                            do! SplinterBots.claimDaily logToObserver getToken config 
                            ApplicationUpdater.checkForUpdates ()
                        | ExecutionModes.Fight -> 
                            do! SplinterBots.playBattle logToObserver getToken config 
                    with 
                    |  :? System.Exception as exn -> 
                        ()
            })
    member this.Subscribe onNext =
        let wrappedAction status = 
            onNext status |> Async.RunSynchronously
        let observer = System.Reactive.Observer.Create (Action<Async<string * SplinterlandStatus>> wrappedAction)
        observers.Add observer 
        new Unsubscriber(observers, observer) :> IDisposable

    member this.Scheduler () =  
        let shouldStartTransfers () =
            let config = Config.getConfiguration ()
            let expresion = Cronos.CronExpression.Parse(config.timers.executeTransfer)
            let now = DateTime.UtcNow
            let occurence = expresion.GetNextOccurrence(now)
            let timeToOccurence = now - occurence.Value
            timeToOccurence.TotalMinutes < 16.0 && timeToOccurence.TotalMinutes  > -16.0

        if shouldStartTransfers ()
        then 
            processor.Post ExecutionModes.Claim
        else 
            processor.Post ExecutionModes.CheckDetails

    member this.ResponseToClaimProcessRequest () =
        processor.Post ExecutionModes.Claim
    
    member this.ResponseToFightProcessRequest () =
        processor.Post ExecutionModes.Fight
    

    member this.ResponseToDetailsProcessRequest () =
        processor.Post ExecutionModes.CheckDetails
