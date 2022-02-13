module Top

open Terminal.Gui
open System
open Functional.SplinterBots.Config

let addTimeout time handler =
    let guiHandler loop = 
        handler()
        true
    Application.MainLoop.AddTimeout(time, Func<MainLoop, bool> guiHandler)

let removeTimeout timeout =
    Application.MainLoop.RemoveTimeout timeout

let showQuitMessage () =
    MessageBox.Query (50, 7, ustr "Exit Splinterbots", ustr "Are you sure you want to quit SplintetrBots?", ustr "Yes", ustr "No") = 0

let quit () =
    if (showQuitMessage()) then Application.Top.Running <- false
