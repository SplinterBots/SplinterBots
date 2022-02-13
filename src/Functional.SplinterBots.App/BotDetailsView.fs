module BotDetailsView

open Terminal.Gui
open Functional.SplinterBots
open System

[<Literal>]
let size = 7

type BotDetailsView (detail, index) as self = 
    inherit FrameView()

    let progress = 
        let progress = createProgress ()
        progress.Id <- ustr detail.username
        progress.X <- Pos.At 1
        progress.Y <- Pos.At 2
        progress.Width <- Dim.Fill 1
        progress.Height <- Dim.Fill 1
        progress.Visible <- false
        progress
    let progressLabel = createLabel "Current Progress: " |> keepAtLine 3 |> hide 
    let progressValue = createLabel "" |> moveToRight progressLabel |> keepAtLine 3 |> hide 
    let decLable = createLabel "DEC: " |> keepAtLine 1
    let decValue = createLabel "" |> moveToRight decLable |> keepAtLine 1
    let spsLabel = createLabel " | SPS: " |> moveToRight decValue |> keepAtLine 1
    let spsValue = createLabel "" |> moveToRight spsLabel  |> keepAtLine 1
    let creditsLabel = createLabel " | Credits: " |> moveToRight spsValue |> keepAtLine 1
    let creditsValue = createLabel "" |> moveToRight creditsLabel |> keepAtLine 1
    let ratingLabel = createLabel "Rating: " |> keepAtLine 2
    let ratingValue = createLabel "" |> moveToRight ratingLabel |> keepAtLine 2
    let powerLabel = createLabel " | Power: " |> moveToRight ratingValue |> keepAtLine 2
    let powerValue = createLabel "" |> moveToRight powerLabel |> keepAtLine 2
    let rankingLabel = createLabel " | Ranking: " |> moveToRight powerValue |> keepAtLine 2
    let rankingValue = createLabel "" |> moveToRight rankingLabel |> keepAtLine 2
    let ecrLabel = createLabel " | ECR: " |> moveToRight rankingValue |> keepAtLine 2
    let ecrValue = createLabel "" |> moveToRight ecrLabel |> keepAtLine 2
    let seasonalLabel = createLabel "Seasonal reward: " |> keepAtLine 3
    let seasonalValue = createLabel "" |> moveToRight seasonalLabel |> keepAtLine 3
    let questLabel = createLabel " | Quest reward: " |> moveToRight seasonalValue |> keepAtLine 3
    let questValue = createLabel "" |> moveToRight questLabel |> keepAtLine 3
    let questNameLabel = createLabel " | Quest Name: " |> moveToRight questValue |> keepAtLine 3
    let questNameValue = createLabel "" |> moveToRight questNameLabel |> keepAtLine 3
    let updateDateLabel = createLabel "Last checked at: "  |> keepAtLine 4
    let updateDateValue = createLabel (DateTime.Now.ToString("HH:mm dd/MM/yyyy")) |> moveToRight updateDateLabel |> keepAtLine 4
    let mutable timer = new obj()

    let addControls () = 
        self.Text <- ustr detail.username
        self.Id <- ustr detail.username
        self.X <- Pos.At 1
        self.Y <- Pos.At (index * size)
        self.Width <- Dim.Fill() - Dim.Sized 2
        self.Height <- Dim.Sized size
        
        self.Add decLable 
        self.Add decValue 
        self.Add spsLabel 
        self.Add spsValue 
        self.Add creditsLabel 
        self.Add creditsValue 
        self.Add ratingLabel 
        self.Add ratingValue 
        self.Add powerLabel 
        self.Add powerValue 
        self.Add seasonalLabel 
        self.Add seasonalValue 
        self.Add questLabel 
        self.Add questValue 
        self.Add rankingLabel 
        self.Add rankingValue 
        self.Add questNameLabel 
        self.Add questNameValue 
        self.Add ecrLabel
        self.Add ecrValue
        self.Add updateDateLabel
        self.Add updateDateValue
        self.Add progress
        self.Add progressLabel
        self.Add progressValue
    let setLabelsVisiblity visibility = 
        decLable.Visible <- visibility
        decValue.Visible <- visibility
        spsLabel.Visible <- visibility
        spsValue.Visible <- visibility
        creditsLabel.Visible <- visibility
        creditsValue.Visible <- visibility
        ratingLabel.Visible <- visibility
        ratingValue.Visible <- visibility
        powerLabel.Visible <- visibility
        powerValue.Visible <- visibility
        rankingLabel.Visible <- visibility
        rankingValue.Visible <- visibility
        ecrLabel.Visible <- visibility
        ecrValue.Visible <- visibility
        seasonalLabel.Visible <- visibility
        seasonalValue.Visible <- visibility
        questLabel.Visible <- visibility
        questValue.Visible <- visibility
        questNameLabel.Visible <- visibility
        questNameValue.Visible <- visibility
        updateDateLabel.Visible <- visibility
        updateDateValue.Visible <- visibility
        
    do 
        addControls ()
        self.UpdateDetail detail 

    member this.GetUsername () =
        detail.username

    member this.SetLoading () =
        show progress |> ignore
        show progressLabel |> ignore
        show progressValue |> ignore
        timer <- Top.addTimeout (TimeSpan.FromMilliseconds(100.)) this.Pulse
        setLabelsVisiblity false        

    member this.Pulse () = 
        progress.Pulse()

    member this.UpdateDetail detail  =
        decValue.Text <- ustr (detail.dec.ToString())
        spsValue.Text <- ustr (detail.sps.ToString())
        creditsValue.Text <- ustr (detail.credits.ToString())
        ratingValue.Text <- ustr (detail.rating.ToString())
        powerValue.Text <- ustr (detail.power.ToString())
        rankingValue.Text <- ustr (detail.leauge.ToString())
        ecrValue.Text <- ustr (detail.ecr.ToString("00.00") + "%")
        seasonalValue.Text <- ustr (detail.sesonalReward.ToString())
        questValue.Text <- ustr (detail.dailyReward.ToString())
        questNameValue.Text <- ustr (detail.dailyQuestName)
        updateDateValue.Text <- ustr (DateTime.Now.ToString("HH:mm dd/MM/yyyy"))

    member this.UpdateProgress status =
        progressValue.Text <- ustr (status.ToString())

    member this.FinishedProcessing () =
        Top.removeTimeout timer |> ignore
        hide progress |> ignore
        hide progressLabel |> ignore
        hide progressValue |> ignore
        setLabelsVisiblity true
