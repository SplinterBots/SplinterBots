namespace Functional.SplinterBots.Cards

open System.Text.Json.Serialization

module Cards =

    open System
    open Functional.SplinterBots.API
    open FsHttp.DslCE
    open System.Text.Json
    open System.IO

    type SimpleUnionConverter<'T> (defaultReadValue: 'T) =
        inherit JsonConverter<'T> ()

        override this.Read (reader, typeToConvert, options) =
            let stringValue = reader.GetString ()
            let unionValue = UnionTools.fromString<'T> defaultReadValue stringValue
            unionValue

        override x.Write (writer, value, options) =
            let stringValue = UnionTools.toString value
            writer.WriteStringValue stringValue

    type CardType =
        | Monster
        | Splinter

    type CardColour =
        | All
        | Red
        | Blue
        | Green
        | White
        | Black
        | Gold
        | Gray

    [<Flags>]
    type CardRarity  =
        | Common = 1
        | Rare = 2
        | Epic = 3
        | Legendary = 4

    type Card =
        {
            id: int
            name: string
            ``type``: CardType
            color: CardColour
            rarity: CardRarity
        }

    [<Literal>]
    let cardsPath = "cards.json"

    let private options =
        let opt = new JsonSerializerOptions ()
        opt.Converters.Add(SimpleUnionConverter<CardColour> CardColour.All)
        opt.Converters.Add(SimpleUnionConverter<CardType> CardType.Monster)
        opt

    let private downloaCardsDetails () =
        async {
            let cardsUri = $"{api2Uri}/cards/get_details"
            let! rawCardsData = executeApiCallToString cardsUri

            do! File.WriteAllTextAsync(cardsPath, rawCardsData) |> Async.AwaitTask
        }
    let ensureCardsListIsDownloaded () =
        async {
            if File.Exists cardsPath
            then
                ()
            else
                do! downloaCardsDetails ()
        }

    let getCards () =
        async {
            let! fileContent = File.ReadAllTextAsync cardsPath |> Async.AwaitTask
            return JsonSerializer.Deserialize<Card seq> (fileContent, options)
        }

    let filterCardsByRarity rarity cards =
        cards
        |> Seq.filter (fun card -> card.rarity = rarity)
