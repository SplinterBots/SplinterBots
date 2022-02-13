module Browser

open Support
open Types
open PuppeteerSharp
open System

let getBrowser headless = 
    async {
        let browserFetcher = new BrowserFetcher(new BrowserFetcherOptions())
        browserFetcher.DownloadAsync() 
            |> Async.AwaitTask 
            |> Async.RunSynchronously
            |> ignore
        let options = new LaunchOptions (Headless = headless)
        return! (Puppeteer.LaunchAsync(options) |> Async.AwaitTask)
    }

let getNewPage (browser: Browser) = 
    async {
        let! page = browser.NewPageAsync() |> Async.AwaitTask
        return page
    }

let closeBrowser (context: Browser) =
    async {
        do! context.CloseAsync() |> Async.AwaitTask
    }

let closePage (context: Context) =
    context |> handleTask (fun ctx -> ctx.CloseAsync())

let goTo url (context: Context) = 
    context |> handleTaskOf (fun ctx -> ctx.GoToAsync(url))

let setViewPortSize width height (context: Context) = 
    let options = new ViewPortOptions (Width = width, Height = height)
    context |> handleTask (fun ctx -> ctx.SetViewportAsync(options))

let setDefaultTimeout timeout (context: Context) =
    async {
        context.DefaultTimeout <- timeout
    }

let click selector (context: Context) =
    context |> handleTaskWithSelector selector (fun ctx -> ctx.ClickAsync())
let clickAll selector (context: Context) =
    let isVisible (element: ElementHandle) = 
        element.IsIntersectingViewportAsync() |> Async.AwaitTask |> Async.RunSynchronously
    let click (element: ElementHandle) = 
        element.ClickAsync()  |> Async.AwaitTask |> Async.RunSynchronously
    async {
        let! elements = context.QuerySelectorAllAsync selector |> Async.AwaitTask
        elements
        |> Seq.filter isVisible
        |> Seq.iter click
    }

let selectOption selector optionName (context: Context) = 
    let values = [| String.toString optionName |]
    context |> handleTaskOfWithSelector selector (fun ctx -> ctx.SelectAsync(values))

let ``type`` selector text (context: Context) =
    context |> handleTaskWithSelector selector (fun ctx -> ctx.TypeAsync(text))

let readValue selector (context: Context) = 
    async {
        let! value = context.WaitForSelectorAsync(selector) |> Async.AwaitTask
        let! valueInJson = value.EvaluateFunctionAsync("el => el.textContent") |> Async.AwaitTask
        let result = valueInJson.ToString()

        return result
    }
let readValueAll selector (context: Context) = 
    let extractValueFromElementHandle (elementHandle: ElementHandle) =
        let valueInJson = 
            elementHandle.EvaluateFunctionAsync("el => el.textContent") 
            |> Async.AwaitTask
            |> Async.RunSynchronously
        valueInJson.ToString()
    async {
        let! values = context.QuerySelectorAllAsync(selector) |> Async.AwaitTask
        let results = values |> Seq.map extractValueFromElementHandle
    
        return results
    }

let exists selector (context: Context) =
    async {
        let! value = context.QuerySelectorAsync selector |> Async.AwaitTask

        return 
            match value with 
            | null -> false
            | _ -> value.IsIntersectingViewportAsync() |> Async.AwaitTask |> Async.RunSynchronously
    }

let queryElements selector (context: Context) = 
    async {
        let! elements = context.QuerySelectorAllAsync selector |> Async.AwaitTask
        return elements |> Array.ofSeq
    }
let readProperty selector property (context: Context) = 
    async {
        let! elements = context.QuerySelectorAllAsync selector |> Async.AwaitTask
        let results = 
            elements 
            |> Seq.map (fun ctx -> ctx.GetPropertyAsync property |> Async.AwaitTask |> Async.RunSynchronously)
            |> Seq.map (fun prop -> prop.RemoteObject.Value.ToString())
            |> Array.ofSeq
        return results
    }

let pressKey (key: Keys) (context: Context) =
    context |> handleTask (fun ctx -> ctx.Keyboard.PressAsync(key.ToString()))

let evaluate javascript (context: Context) =
    context |> handleTaskOf (fun ctx -> ctx.EvaluateExpressionAsync(javascript))

let evaluateAndReturn javascript (context: Context) =
    async {
        let! result = context.EvaluateExpressionAsync(javascript)  |> Async.AwaitTask
        return result.ToString()
    }

let closeConfirmationDialogWhenAppear (context: Context) =
    evaluate "window.confirm = () => true" context

let waitForXSeconds timeout (context: Context) =
    let timeoutInMiliseconds = int(TimeSpan.FromSeconds(timeout).TotalMilliseconds)
    context |> handleTask ( fun ctx -> ctx.WaitForTimeoutAsync(timeoutInMiliseconds))
let waitFor5Seconds (context: Context) =
    waitForXSeconds 5.0 context
let waitForASecond (context: Context) =
    waitForXSeconds 1.0 context
