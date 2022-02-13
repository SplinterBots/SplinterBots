module Errors

open System

type Error = 
    {
        reportedOn: DateTime
        user: string
        stackTrace: string 
        message: string
    }
module Error =
    let bind user (exn: exn) =
        {
            reportedOn = System.DateTime.UtcNow.Date
            user = user
            message = exn.Message
            stackTrace = exn.StackTrace
        }
