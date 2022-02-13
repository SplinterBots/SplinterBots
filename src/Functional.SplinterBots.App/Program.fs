open Terminal.Gui
open System
open Functional.SplinterBots
open Functional.SplinterBots.Cards
open Functional.SplinterBots.Config
open Functional.SplinterBots.Storage.WatchMessages
open System.Diagnostics

let getUsername (config: SplinterBotsCofig) =
    config.transferDetails
    |> Seq.map (fun d -> d.username)

let private schedulerTime = (TimeSpan.FromMinutes(30.0))

let Main () =
    ApplicationUpdater.checkForUpdates ()

    printf "Starting SplinterBots\n"
    let config = Config.getConfiguration()

    printf "Gettings cards from Splinterland\n"
    Cards.ensureCardsListIsDownloaded () |> Async.RunSynchronously

    printf "Startingh the UI\n"

    try
        Application.Init()

        let worker = BackgroundWorker.BackgroundWorker()

        let botWindow = new BotsWindow.BotsWindow (getUsername config)
        let configWindow = new ConfigurationWindow.ConfigurationWindow ()
        //let cardsWindow = new CardsWindow.CardsWindow ()

        let timer = Top.addTimeout schedulerTime worker.Scheduler
        use obseravableSubscrition = worker.Subscribe botWindow.UpdateStatus
        use databaseObserver = worker.Subscribe saveMessagesToDatabase
        let top = Application.Top
        let statusBar =
            Toolbars.statusBar
                configWindow.Show
                configWindow.Show
                //cardsWindow.Show
                worker.ResponseToClaimProcessRequest

        top.Add botWindow
        top.Add (Toolbars.menu configWindow.Show, statusBar)

        worker.ResponseToDetailsProcessRequest ()

        Application.Run ()
        0
    with
        | :? NullReferenceException -> 1
        | :? System.Exception as exp  -> 2

module SplinterBotsApp =
    [<EntryPoint>]
    let main _ =
        Main ()
