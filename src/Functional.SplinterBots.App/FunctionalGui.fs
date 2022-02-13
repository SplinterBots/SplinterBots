[<AutoOpen>]
module FunctionalGui

open Terminal.Gui
open NStack
open System

[<AutoOpen>]
module Operations  =     
    let ustr (x: string) = ustring.Make(x)

    let addTo (window: #View) (item: #View)=
        window.Add item
        item

    let private setVisibility visibilty (view: #View) =
        view.Visible <- visibilty
        view
    let hide (view: #View) =
        setVisibility false view
    let show (view: #View) =
        setVisibility true view
        
[<AutoOpen>]
module Position = 
    let abstractPosition x y (view: #View) =
        view.X <- Pos.At x
        view.Y <- Pos.At y
        view
    let moveToCenter (view: #View) =
        view.X <- Pos.Center()
        view
    let moveToRight parent (view: #View) =
        view.X <- Pos.Right parent
        view
    let moveToLeft parent (view: #View) =
        view.X <- Pos.Left parent
        view
    let moveToBottom parent (view: #View) =
        view.X <- Pos.Bottom parent
        view
    let moveTo positionX  (view: #View) =
        view.X <- Pos.At positionX
        view
        
    let keepAtLine positionY (view: #View) =
        view.Y <- Pos.At positionY
        view
        
    let keepAtBottom parent (view: #View) =
        view.Y <- Pos.Bottom parent
        view

[<AutoOpen>] 
 module Controls =  
    let createWindow name = 
        let item = new Window (ustr name)
        item.LayoutStyle <- LayoutStyle.Computed
        item 
    let hideWindow (window: Window) =
        window.Visible <- false
        window
    let showWindow (window: Window) =
        window.Visible <- true
        window
    let setFocus (window: #View) =
        window.SetFocus()
        window

    let toggleVisibility (item: #View) =
        item.Visible <- not item.Visible
        item

    let setWidth size (item: #View) =
        item.Width <- size
        item
    let fillWidth (item: #View) =
        setWidth (Dim.Fill 1) item

    let setHeight size (item: #View) =
        item.Height <- size
        item
    let fillHeight (item: #View) =
        setHeight (Dim.Fill 1) item

    let fillWidthAndHeight (item: #View) =
        item
        |> fillHeight
        |> fillWidth 

    [<AutoOpen>]
    module Label = 
        let createLabel text = 
            let label = new Label (ustr (text.ToString())) 
            label.TextAlignment <- TextAlignment.Left
            label  
        let colorScheme color (item: #Label) = 
            item.ColorScheme <- color 
            item

    let createCheckBox text = 
        new CheckBox(ustr text)
    let setState setChecked (item: CheckBox) = 
        item.Checked <- setChecked
        item

    [<AutoOpen>]
    module Button =
        let createButton text = 
            new Button(ustr text)
        let addClick action (item: Button)  = 
            item.add_Clicked (Action action)
            item
        let setDefaultButton (item: Button) = 
            item.IsDefault <- true
            item

    let createTimeField text = 
        new TimeField()
    let setTime time (item: TimeField) =
        item.Time <- time
        item

    module TextBox = 
        let createTextBox text = 
            new TextField(ustr text)
        let setText text (item: TextField) = 
            item.Text <- ustr (text.ToString())
            item
        let allowNumberOnly (item: TextField) =
            let allowOnlyNumbers (args: TextChangingEventArgs) = 
               let (canParseToNumber, dt) = Decimal.TryParse (args.NewText.ToString())
               args.Cancel <- (not canParseToNumber)
            item.add_TextChanging(Action<TextChangingEventArgs> allowOnlyNumbers)
            item
        let limitCharactersTo characterLimits (item: TextField) =
            let limitCharacter (args: TextChangingEventArgs) = 
               args.Cancel <- not (args.NewText.Length <= characterLimits)
            item.add_TextChanging(Action<TextChangingEventArgs> limitCharacter)
            item

    let createStatusItem key text action = StatusItem(key, ustr text, Action action)
    let noKey = Key.Null
    let noAction () = ()

    let setContentSize contentWidth contentHeight (item: #ScrollView) =
        item.ContentSize <- new Size (contentWidth, contentHeight)
        item 

    let createScrollView contentWidth contentHeight =
        let scroll = new ScrollView ()
        setContentSize contentWidth contentHeight scroll
    let showVerticalScroll (scroll:ScrollView) =
        scroll.ShowVerticalScrollIndicator <- true
        scroll
    let showHorizontalScroll (scroll:ScrollView) =
        scroll.ShowHorizontalScrollIndicator <- true
        scroll

    let createFrameView name = 
        let view = new FrameView (ustr name)
        view.LayoutStyle <- LayoutStyle.Computed
        view

    let createProgress () =
        new ProgressBar()

    module ListView =
        type SystemList<'T> = System.Collections.Generic.List<'T>
        let createListView<'T> (items: 'T seq) = 
            new ListView (new SystemList<'T>(items))
        let setSource (items: 'T seq) (view: ListView) =
            view.SetSource (new SystemList<'T>(items))
            view
        let allowsMarking (view: ListView) =
            view.AllowsMarking <- true
            view
        let allowsMultipleSelection (view: ListView) =
            view.AllowsMultipleSelection <- true
            view
        let addSelectedItemAction action (view: ListView) =
            view.add_SelectedItemChanged (Action<ListViewItemEventArgs> action)
            view

    module ComboBox = 
        type SystemList<'T> = System.Collections.Generic.List<'T>
        let createComboBox<'T> name  = 
            new ComboBox(ustr name) 
        let setSource (items: 'T seq) (comboBox: ComboBox) =
            comboBox.SetSource (new SystemList<'T>(items))
            comboBox

[<AutoOpen>]
module Size =
    let withWidth size (item: #View) =
        item.Width <- size
        item

    let percentageSize value =
        Dim.Percent (float32(value))

    let fixedSize value = 
        Dim.Sized value

    let fillHalf = percentageSize 50.0 
    let fillThird = percentageSize 33.0 
    let fillQuater = percentageSize 25.0 

    let fillFull = Dim.Fill 1

    let fill margin =
        Dim.Fill(margin)

    
    let takeHalfScreeDim = Dim.Percent (float32(30))
     


