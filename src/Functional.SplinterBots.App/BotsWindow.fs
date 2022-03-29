module BotsWindow

open Terminal.Gui
open Functional.SplinterBots

type BotsWindow (usernames) as self =
    inherit Window()
    
    let fightButton = 
        createButton "Fight"
        |> abstractPosition 1 0
    let startClaimProcesButton = 
        createButton "Start claim process"
        |> moveToRight fightButton
    let configuration =
        createButton "Configuration"
        |> moveToRight startClaimProcesButton
    let exit =
        createButton "Exit"
        |> moveToRight configuration
        
    let scroll = 
        createScrollView 100 6
        |> abstractPosition 0 1
        |> fillWidthAndHeight
    let bots =
        let bots  = 
            usernames 
            |> Seq.map (fun username -> {AccountDetail.notLoaded with username = username })
            |> Seq.mapi (fun index detail -> new BotDetailsView.BotDetailsView (detail, index))
        
        scroll.ContentSize <- new Size(100, (Seq.length bots) * BotDetailsView.size)
        scroll.Add (bots |> Seq.cast<View> |> Array.ofSeq)
        Map<Username, BotDetailsView.BotDetailsView>(bots |> Seq.map (fun bot -> (bot.GetUsername(), bot))) 
    
    let setAccountDetails (detail: AccountDetail) =
        bots.[detail.username].UpdateDetail detail
        bots.[detail.username].FinishedProcessing ()
    let indicateLoading username = 
        bots.[username].SetLoading ()
    let enableProcessed username =
        bots.[username].FinishedProcessing ()
    let updateStatus username status =
        bots.[username].UpdateProgress status

    do
        let controls =
            bots.Values
            |> Seq.map (fun bot -> bot)
            |> Seq.cast<View>
            |> Array.ofSeq
        
        scroll.Add controls
        self.Add fightButton
        self.Add startClaimProcesButton
        self.Add configuration 
        self.Add exit 
        self.Add scroll
        
    member this.UpdateStatus status =
        async {
            let! (user, status) = status
            
            match status with 
            | StartedProcessing username -> 
                indicateLoading username
                updateStatus user "Starting Loading"
            | LoadedAccountDetails details -> 
                setAccountDetails details
                updateStatus user "seting account details"
            | Processed username -> 
                enableProcessed username
                updateStatus user "Finished"
            | AllowToFail msg -> 
                updateStatus user msg
            | FinishedProcessing -> 
                bots
                |> Seq.iter (fun pair -> pair.Value.FinishedProcessing () |> ignore)
            | _ -> 
                updateStatus user status
        }

    member this.SetFightAction action = 
        fightButton
        |> addClick action
        |> ignore
    member this.SetClaimAction action = 
        startClaimProcesButton 
        |> addClick action
        |> ignore
    member this.SetConfigurationAction action = 
        configuration
        |> addClick action
        |> ignore
    member this.SetExitAction action = 
        exit
        |> addClick action
        |> ignore
