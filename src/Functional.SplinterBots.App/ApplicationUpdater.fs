module ApplicationUpdater

open System
open System.Diagnostics

let checkForUpdates () =
    if not Debugger.IsAttached
    then 
        printf $"Checking for updates \n, current version {Version.getVersion () |> Version.toString}"

        let applicationLocation = AppContext.BaseDirectory
        let updateApplication =
            GitHubReleaseUpdate.updateApplication applicationLocation 
            |> Async.RunSynchronously

        if updateApplication 
        then 
            Process.Start "update.exe" |> ignore
            Environment.Exit 0
