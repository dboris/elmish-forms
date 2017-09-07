[<AutoOpen>]
module Fable

open System

[<AutoOpen>]
module PowerPack = ()
[<AutoOpen>]
module Core = 
    [<AutoOpen>] 
    module JsInterop = ()
[<AutoOpen>]
module Import =
    [<AutoOpen>] 
    module Browser =
        [<AutoOpen>]
        module console =
            let error (str, ex) = sprintf "%s: %O" str ex |> Diagnostics.Debug.WriteLine
            let log o = sprintf "%s -- %A" (DateTime.Now.ToString("o")) o |> Diagnostics.Debug.WriteLine
            let toJson o = o
    [<AutoOpen>] 
    module JS =
        [<AutoOpen>]
        module JSON =
            let parse str = str
        type Promise<'T>() = 
            class end
            with
                static member map _ = failwith "Promise not supported"
                static member catch _ = failwith "Promise not supported"
