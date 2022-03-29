module ConfigurationWindow

open FSharp.Configuration
open Terminal.Gui
open DefaultValues
open Functional.SplinterBots
open FunctionalGui.Controls.TextBox

type SplinterBotConfig = YamlConfig<"config.yml">

type ConfigurationWindow () as self =
    inherit Window(ustr "Configuration")

    let claimDailyRewardCheckbox = createCheckBox "Claim _Daily Reward"
    let claimSessonalRewardCheckbox = createCheckBox "Claim S_esonal Reward"
    let buyCardsWithCredit = createCheckBox "Buy cards with crediits"
    let transferCardsCheckbox = createCheckBox "_Transafer Cards"
    let rentCardsCheckbox = createCheckBox "_Rent card to reach bronze 2"
    let startTransferTime = createTimeField ()
    let decLimit = 
        createTextBox "" 
        |> setWidth (Dim.Sized 12) 
        |> moveToFixedLine 5
        |> allowNumberOnly
        |> limitCharactersTo 8
    let saveButton = createButton "Save"
    let closeButton = createButton "Cancel"

    let save () =
        let configFile = SplinterBotConfig ()
       
        configFile.claimDailyReward <- claimDailyRewardCheckbox.Checked
        configFile.claimSeasonReward <- claimSessonalRewardCheckbox.Checked
        configFile.rentCards <- rentCardsCheckbox.Checked
        configFile.buyCardWithCredits <- buyCardsWithCredit.Checked
        configFile.transferCardsToMainAccount <- transferCardsCheckbox.Checked
        let executionTime = startTransferTime.Time
        configFile.timers.executeTransfer <- $"{executionTime.Minutes} {executionTime.Hours} * * *"
        configFile.decLimit <- float(decLimit.Text.ToString())

        configFile.Save("config.yml")
    let cronToTimeSpan cron =
        let expression = Cronos.CronExpression.Parse(cron)
        let now = System.DateTime.UtcNow
        let nextOccurance = expression.GetNextOccurrence(now).Value
        nextOccurance.TimeOfDay
    let closeWindow () = 
        Application.Top.Remove self

    do 
        self.Add (claimDailyRewardCheckbox |> moveToFixedLine 0)
        self.Add (claimSessonalRewardCheckbox |> moveBelow claimDailyRewardCheckbox)
        self.Add (transferCardsCheckbox |> moveBelow claimSessonalRewardCheckbox )
        self.Add (rentCardsCheckbox |> moveBelow transferCardsCheckbox)
        self.Add (
            createLabel "Set start time of resources transfer:" 
            |> moveBelow rentCardsCheckbox
            |> moveTo 15)
        self.Add (startTransferTime |> moveBelow rentCardsCheckbox)
        self.Add decLimit
        self.Add (
            createLabel "Transfer DEC above this limit" 
            |> moveBelow startTransferTime
            |> moveTo 15)
        self.Add (
            saveButton 
            |> addClick (save >> closeWindow)  
            |> setDefaultButton 
            |> moveBelow startTransferTime)
        self.Add (
            closeButton 
            |> addClick closeWindow 
            |> moveBelow startTransferTime
            |> moveToRight saveButton)

    member this.Show () =
        let config = Config.getConfiguration()

        claimDailyRewardCheckbox |> setState config.claimDailyReward |> ignore
        claimSessonalRewardCheckbox |> setState config.claimSeasonReward |> ignore
        rentCardsCheckbox |> setState config.rentCards |> ignore
        buyCardsWithCredit |> setState config.buyCardWithCredits |> ignore
        transferCardsCheckbox |> setState config.transferCardsToMainAccount |> ignore
        startTransferTime |> setTime (cronToTimeSpan config.timers.executeTransfer) |> ignore
        decLimit |> setText config.decLimit |> ignore

        Application.Top.Add self
        setFocus self |> ignore
