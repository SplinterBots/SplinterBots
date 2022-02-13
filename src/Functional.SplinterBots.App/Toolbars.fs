module Toolbars 

open System
open Terminal.Gui
open DefaultValues


let menu showConfiguration = 
    new MenuBar (
    [|
        new MenuBarItem(
            ustr "_Exit",
            ustr "",
            Top.quit)
        new MenuBarItem(
            ustr "_Settings",
            ustr "",
            Action showConfiguration)
        //new MenuBarItem(
        //    ustr "_Account Settings",
        //    ustr "",
        //    ConfigurationWindow.show)
        new MenuBarItem(
            ustr $"Current version: {Version.getVersion () |> Version.toString}",
            ustr "",
            fun () -> ())
    |])

let statusBar showConfiguration showCards runClaim = 
    new StatusBar (
        [|
            createStatusItem (Key.AltMask ||| Key.S)  "~ALT+S~ Settings" showConfiguration
            //createStatusItem (Key.AltMask ||| Key.A)  "~ALT+A~ Accounts settings" ConfigurationWindow.show
            createStatusItem (Key.AltMask ||| Key.R)  "~ALT+R~ Rent settings" showCards

            //createStatusItem (Key.AltMask ||| Key.D)  "~ALT+D~ Run Details" runDetails
            createStatusItem (Key.AltMask ||| Key.C)  "~ALT+C~ Run Claim" runClaim
            //createStatusItem (Key.AltMask ||| Key.B)  "~ALT+B~ Run Rent" runRent
        |]) 
