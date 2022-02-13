namespace Functional.SplinterBots

module Config = 
    open System    
    open Microsoft.Extensions.Configuration
    open Microsoft.Extensions.Configuration.UserSecrets

    type DestinationAccountName = string
    type Username = string
    type PostingKey = string
    type ActiveKey = string
    
    type UserConfig () = 
        member val username = "" with get,set
        member val postingKey = "" with get,set
        member val activeKey = "" with get,set

    type TransferDetails = 
       {
           destinationAccount: DestinationAccountName 
           username: Username 
           activeKey: ActiveKey
           postingKey: PostingKey
       }
    module TransferDetails = 
        let bind destinationAccount (loginDetails: UserConfig) = 
           {
               destinationAccount = destinationAccount
               username = loginDetails.username
               activeKey = loginDetails.activeKey
               postingKey = loginDetails.postingKey
           }

    type Browser () = 
        member val headless = true with get,set
        member val parallelExecution = 3 with get,set

    type Timers () =
        member val executeTransfer =  "30 20 * * *" with get,set
        member val checkStatus = "*/10 * * * *" with get,set

    type SplinterBotsCofig = 
        {
            timers: Timers
            claimDailyReward: bool
            claimSeasonReward: bool            
            rentCards: bool
            desiredLeague: SplinterlandLeague
            transferDetails: TransferDetails array
            decLimit: decimal
            cards: string array
            parallelExecution: int
        }

    let private getSectionAs<'T> sectionName (config: IConfigurationRoot) =
       config.GetSection(sectionName).Get<'T>()

    let private defaultArg defaultValue value = 
        match value with 
        | null -> defaultValue
        | _ -> value

    let getConfiguration ()  =
        let builder  = 
            new ConfigurationBuilder()
                |> fun config -> config.AddYamlFile "config.yml"
                |> fun config -> config.AddYamlFile ("accounts.yml", true)
                |> fun config -> config.AddYamlFile ("cards.yml", true)
                |> fun config -> config.AddEnvironmentVariables()
        let intermediate  = builder.Build()
        let userSecretId = 
            match intermediate.["userSecretId"] with 
            | null -> "missing"
            | _ as x -> x
        builder
            |> fun config -> config.AddUserSecrets(userSecretId)
            |> fun config -> config.Build()
            |> fun config -> 
                let transferSettigs = 
                    let sentTo = config.GetValue<string>("sentTo")
                    defaultArg [||] (config.GetSection("accounts").Get<UserConfig array>())
                    |> Array.map (fun userInfo -> TransferDetails.bind sentTo userInfo)
                    |> Array.sortByDescending (fun item -> item.username)
                    |> Array.ofSeq
                {
                    claimDailyReward = config.GetValue<bool> "claimDailyReward" 
                    claimSeasonReward = config.GetValue<bool> "claimSeasonReward"
                    rentCards = config.GetValue<bool> "rentCards"
                    desiredLeague = config.GetValue<SplinterlandLeague> "desiredLeague"
                    parallelExecution = config.GetValue<int> "parallelExecution"
                    transferDetails = transferSettigs
                    timers = config |> getSectionAs "timers"
                    decLimit = config.GetValue<decimal> "decLimit"
                    cards =  defaultArg [||] (config.GetSection("cards").Get<string array>())
                }
