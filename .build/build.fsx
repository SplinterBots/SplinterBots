#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Target //"
#r "paket: nuget Fake.IO.Zip //"
#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

Target.initEnvironment ()

let runtime = Environment.environVarOrDefault "runtime" "win-x64"
let version = Environment.environVarOrDefault "version" "0001.01"

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ "./artifacts"
    ++ "./temp"
    |> Shell.cleanDirs 

    Directory.ensure "./artifacts"
    Directory.ensure $"./temp/{runtime}"
)

Target.create "Build" (fun _ ->
    let buildOptions (options: DotNet.BuildOptions) = 
        {
            options with
                Runtime = Some runtime;
                Configuration = DotNet.BuildConfiguration.Release;
        }
    !! "src/**/*.fsproj"
    |> Seq.iter (DotNet.build buildOptions)
)

let publishOptions (options: DotNet.PublishOptions) = 
    { 
        options with
            Runtime = Some runtime;
            Configuration = DotNet.BuildConfiguration.Release;
            SelfContained = Some true;
            //VersionSuffix = Some version;
            MSBuildParams =
                { options.MSBuildParams with
                    Properties = [
                        "PublishTrimmed", "true"
                        "EnableCompressionInSingleFileEnableCompressionInSingleFile", "true"
                        "PublishSingleFile", "true"
                        "IncludeNativeLibrariesForSelfExtract", "true"
                        "PublishReadyToRun", "true"
                        "TrimMode", "Link"
                    ]
                }
    }
Target.create "PublishDotnet" (fun _ ->
    !! "src/*.App/*.*proj"
    |> Seq.iter (DotNet.publish publishOptions)

    let files = 
        !! "src/*.App/**/publish/*.*"
        -- "src/*.App/**/publish/*.pdb"
        -- "src/*.App/**/publish/*.xml"
        -- "src/*.App/**/publish/*.json"
        -- "src/*.App/**/publish/createdump.exe"
        ++ "src/*.App/**/publish/../*.so"
        |> Array.ofSeq
    let directory = $"./temp/{runtime}"

    files |> Seq.iter (fun file -> Fake.IO.Shell.copyFile directory file)
)

Target.create "UpdateVersionFile" (fun _ -> 
    Fake.IO.File.writeString false $"./temp/{runtime}/version.info" version
)

Target.create "CreateZip" (fun _ -> 
    let files = 
        !! $"./temp/{runtime}/*.*"
        |> Array.ofSeq

    Fake.IO.Zip.createZip  $"./temp/{runtime}" $"./artifacts/{runtime}-{version}.zip" "" 5 true files
)

Target.create "Publish" ignore
Target.create "All" ignore

"Clean"
  ==> "Build"
  ==> "All"

"Clean"
  ==> "PublishDotnet"
  ==> "UpdateVersionFile"
  ==> "CreateZip"
  ==> "Publish"

Target.runOrDefault "All"
