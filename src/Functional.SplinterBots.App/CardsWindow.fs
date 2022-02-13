module CardsWindow

open FSharp.Configuration
open Terminal.Gui
open Functional.SplinterBots.Cards
open FunctionalGui.Controls.ListView
open FunctionalGui.Controls
open System

type CardsConfig = YamlConfig<"cards-example.yml">

type SystemCardsList = System.Collections.Generic.List<Card>
type CardsDataSource (items: Card seq, selectedItems: Card seq) =
    let items = new SystemCardsList(items)
    let selectedItems = new SystemCardsList(selectedItems)

    interface IListDataSource with 
        member this.IsMarked item =
            let item = items.[item]
            selectedItems.Contains item
        member this.SetMark (item: int, value: bool) =
            let item = items.[item]
            match value with 
            | true -> 
                if not (selectedItems.Contains item)
                then selectedItems.Add item
            | false -> selectedItems.Remove item |> ignore
        member this.ToList () =
            items :> System.Collections.IList
        member this.Count =
            items.Count
        member this.Length =
            40
        member this.Render (container: ListView, driver: ConsoleDriver, selected: bool, item: int, col: int, line: int, width: int, start: int) =
            container.Move (col, line)
            let item = items.[item]
            let text = ustr item.name
            driver.AddStr text
    member this.GetSelected () =
        selectedItems |> Seq.map (fun card -> card.name)
    member this.UpdateItems (newItems: Card seq) =
        items.Clear()
        items.AddRange (newItems |> Seq.sortBy (fun card -> card.name))
    
type CardsWindow () as self =
    inherit Window(ustr "Cards")

    let cards = getCardsData () |> Async.RunSynchronously
    let dataSource = 
        let selectedCards = getSelectedCards () |> Async.RunSynchronously
        CardsDataSource(cards, selectedCards)
    let cardsColoursView = 
        let cardColours = getColours ()
        createListView cardColours
        |> setWidth (fixedSize 20)
        |> setHeight fillFull
        |> abstractPosition 1 0
    let cardsView = 
         new ListView (dataSource)
        |> setWidth (fixedSize 40)
        |> setHeight fillFull
        |> abstractPosition 1 0
        |> allowsMarking
        |> allowsMultipleSelection
        |> moveToRight cardsColoursView
    let selectedCards = 
        createListView [||]
        |> setWidth (fill 2)
        |> setHeight (fill 2)
        |> keepAtLine 1
        |> moveToRight cardsView
    let saveButton =
        createButton "Save"
        |> keepAtLine 0
        |> moveToRight cardsView
    let cancelButton =
        createButton "Cancel"
        |> keepAtLine 0
        |> moveToRight saveButton

    let setSelectedCards () =
        selectedCards |> setSource (dataSource.GetSelected ()) |> ignore

    let setCardForSelection (args: ListViewItemEventArgs) =
        let selectedColour = args.Value :?> string |> UnionTools.fromString CardColour.Red
        dataSource.UpdateItems (cards |> filterByCardColour selectedColour)
        setSelectedCards ()
        
    let updateSelected args =
        setSelectedCards ()

    let save () =
        let selectedCards = CardsConfig ()
        selectedCards.cards <- dataSource.GetSelected() |> Array.ofSeq
        selectedCards.Save("cards.yml")

    let close () =
        Application.Top.Remove self

    do 
        cardsView |> addSelectedItemAction updateSelected |> ignore
        cardsColoursView |> addSelectedItemAction setCardForSelection |> ignore
        
        saveButton |> addClick (save >> close) |> ignore
        cancelButton |> addClick close |> ignore

        self.Add cardsColoursView
        self.Add cardsView 
        self.Add selectedCards
        self.Add saveButton
        self.Add cancelButton

        let scroll = new ScrollBarView (cardsView, true)
        cardsView.add_DrawContent (
            Action<Rect> (
                fun args -> 
                    scroll.Size <- cardsView.Source.Count - 1
                    scroll.Position <- cardsView.TopItem
                    scroll.OtherScrollBarView.Size <- cardsView.Maxlength - 1
                    scroll.OtherScrollBarView.Position <- cardsView.LeftItem
                    scroll.Refresh ()))

    member this.Close() =
        Application.Top.Remove self

    member this.Show () =
        setSelectedCards ()
        Application.Top.Add self
        setFocus self |> ignore
