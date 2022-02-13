module Types

type Keys =
    | Enter
    | Escape
    | Tab

type Context = PuppeteerSharp.Page

type Log = string -> Context -> Async<unit>
