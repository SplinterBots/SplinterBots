open System
open System.Diagnostics
open System.IO

printfn "Begin SplinterBots updating process."

let killSplinterBotsProcess () =
    let killProcess (p: Process) = p.Kill ()

    
    Process.GetProcessesByName("SplinterBots")
    |> Seq.iter killProcess 

let copyFilesFromNewFolder mainPath newFolderPath =
    let moveFiles (sourceFile: string) = 
        let fileName =  Path.GetFileName sourceFile
        let destinationFile = Path.Combine (mainPath , fileName)
        File.Move (sourceFile, destinationFile, true)
    Directory.GetFiles(newFolderPath)
    |> Array.filter (fun file -> file.Contains("Update.exe") <> true)
    |> Array.iter moveFiles

let clearNewFolder newFolderPath =
    Directory.Delete (newFolderPath, true)

let mainPath = AppContext.BaseDirectory
let newFolderPath = Path.Combine (mainPath, "new")

printfn "Checking and killing SplinterBots"
killSplinterBotsProcess ()

printfn "Coping new version of SplinterBots"
copyFilesFromNewFolder mainPath newFolderPath

printfn "Remove newly copired files"
clearNewFolder newFolderPath

printfn "Starting SplinterBots"
Process.Start "SplinterBots.exe" |> ignore
