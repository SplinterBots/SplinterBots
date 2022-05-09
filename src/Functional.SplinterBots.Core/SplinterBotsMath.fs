namespace Functional.SplinterBots

module SplinterBotsMath =
    open System

    let (|LowerThanMinimal|_|) input =
        if input < 0.001M then
            Some true
        else
            None

    let round number =
        Math.Floor(1000.0M * number) / 1000.0M

    let calculateDecDonation decAmount =
        let decAmount = decAmount |> round
        match decAmount with
        | x when x < 0.1M -> None
        | x when x < 1M -> Some 0.05M
        | x when x < 50M -> Some (x * 0.05M)
        | x when x < 100M -> Some (x * 0.04M)
        | x when x < 150M -> Some (x * 0.03M)
        | x when x < 200M -> Some (x * 0.02M)
        | _ -> Some (decAmount * 0.01M)

    let calcualteSPSDonation spsAmount =
        let spsAmount = spsAmount |> round
        match spsAmount with
        | x when x <= 0.001M -> None
        | x when x <= 0.01M -> Some 0.001M
        | _ -> Some (spsAmount * 0.05M)
    let removeComma (input: string) =
        input.Replace(",", "")
    let parseInt (str: string) =
        let (result, value) = Int32.TryParse(str |> removeComma)

        match result with
        | true -> value
        | _ -> 0
    let parseDecimal (str: string) =
        let (result, value) = Decimal.TryParse(str |> removeComma)

        match result with
        | true -> value
        | _ -> 0M
