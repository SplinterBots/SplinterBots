module BotsWindow

open Terminal.Gui
open Functional.SplinterBots

type BotsWindow (usernames) as self =
    inherit Window()

    let statusControl = createLabel ""
    let currentUser = createLabel ""
    let mode = createLabel ""
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
    let setMode text = 
        mode.Text <- ustr (text.ToString()) 
    let updateStatus username status =
        bots.[username].UpdateProgress status
    let setCurrentUser username =
        currentUser.Text <- ustr username
    do
        let controls =
            bots.Values
            |> Seq.map (fun bot -> bot)
            |> Seq.cast<View>
            |> Array.ofSeq
        scroll.Add controls
        self.Add scroll
        
    member this.UpdateStatus status =
        async {
            let! (user, status) = status
            
            match status with 
            | StartedProcessing username -> 
                indicateLoading username
                updateStatus user "Starting Loading"
            | Mode x -> setMode x
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
