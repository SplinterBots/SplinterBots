open Functional.SplinterBots
open Functional.SplinterBots.API
open Functional.SplinterBots.API.Battle

// For more information see https://aka.ms/fsharp-console-apps
printfn "Start gathering data"


let getBattleDetailsForUser username =
    async {
        let! battles = API.Battle.getBattle username 
        let details = 
            battles
            |> Seq.map (fun b -> BattleDetails.bind b.details |> Async.RunSynchronously)
            |> Seq.filter (fun x -> x.IsSome)
            |> Seq.map (fun x -> x.Value)
            
        return details
    }
let selectColours playerName (battleDetails: BattleDetails seq) = 
    battleDetails
    |> Seq.map(fun x -> x.)

let createFile fileData = 
    ()

async {
    let! leaderboard = API.Leaderboard.getLeaderboard Leaderboard.Leauge.Bronze
    let leaderboard = leaderboard  |> Seq.take 1

    leaderboard 
    |> Seq.iter (fun item -> printfn $"User: {item.player} with rank: {item.rank}")

    leaderboard 
    |> Seq.map getBattleDetailsForUser
    |> Seq.map selectColours
} |> Async.RunSynchronously
