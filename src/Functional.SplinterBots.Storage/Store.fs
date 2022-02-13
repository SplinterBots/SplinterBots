namespace Functional.SplinterBots.Storage

module Store =

    open System
    open LiteDB    
    open Microsoft.FSharp.Linq.RuntimeHelpers

    let bindId (id: obj) = 
        match id with 
        | :? int as x-> 
            BsonValue(x)
        | :? DateTime as x-> 
            BsonValue(x)
        | :? string as x-> 
            BsonValue(x)
        | _ -> 
            BsonValue(id.ToString())
    type Store<'T> (dbName: string) = 
        let store = 
            let connectionString = 
                new LiteDB.ConnectionString($"Filename=.\{dbName}.db;Connection=shared")                    
            new LiteDB.LiteDatabase(connectionString)
        member this.Upsert (item: 'T) =
            let _ = store.GetCollection<'T>().Upsert (item)
            ()

        member this.GetAll () =
            store.GetCollection<'T>().FindAll ()

        member this.GetById id =
            let bsonId = bindId id
            store.GetCollection<'T>().FindById (bsonId)
            
        member this.GetSingle (filter: BsonExpression) =
            store.GetCollection<'T>().FindOne filter

        member this.GetFiltered (filter: BsonExpression) =
            store.GetCollection<'T>().Find filter

module Patterns = 
    let (|NotNull|_|) value = 
        if obj.ReferenceEquals(value, null) 
        then None 
        else Some()
