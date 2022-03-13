module AccountsWindow

open System
open FSharp.Configuration
open Terminal.Gui
open DefaultValues
open Functional.SplinterBots
open FunctionalGui.Controls.TextBox

type SplinterBotConfig = YamlConfig<"accounts-example.yml">


let private saveButton = createButton "Save"
let private closeButton = createButton "Cancel"

let private save () =
    let configFile = SplinterBotConfig()
       
  

    configFile.Save("accounts.yml")

let handle = 
    let window = 
        createWindow "Configuration" 
    window.ColorScheme <- Colors.Dialog
    
    let closeWindow () = 
        Application.Top.Remove window

    window.Add (
        saveButton 
        |> addClick (save >> closeWindow)  
        |> setDefaultButton 
        |> moveToFixedLine 8)
    window.Add (
        closeButton 
        |> addClick closeWindow 
        |> moveToFixedLine 8
        |> moveToRight saveButton)

    window

let show () =
    let config = Config.getConfiguration()


    Application.Top.Add handle
    setFocus handle |> ignore
