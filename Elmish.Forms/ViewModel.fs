namespace Elmish.Forms

open System


type PropertyAccessor<'model, 'msg> =
    | Get of Getter<'model>
    | GetSet of Getter<'model> * Setter<'model, 'msg>
    | Cmd of Xamarin.Forms.Command

and ViewModelBase<'model, 'msg>(m : 'model, dispatch, propMap : ViewBindings<'model, 'msg>) as self =

    let propertyChanged = Event<_, _> ()
    let notifyPropertyChanged name = 
        let key = "Item[" + name + "]"
        //console.log <| sprintf "Notify %s" key
        propertyChanged.Trigger(self, ComponentModel.PropertyChangedEventArgs key)

    let mutable model : 'model = m

    let props = new Collections.Generic.Dictionary<string, PropertyAccessor<'model, 'msg>> ()

    let notify (p : string list) =
        p |> List.iter notifyPropertyChanged
        let raiseCanExecuteChanged =
            function
            | Cmd c -> Xamarin.Forms.Device.BeginInvokeOnMainThread(fun () -> c.ChangeCanExecute ())
            | _ -> ()
        props |> List.ofSeq |> List.iter (fun kvp -> raiseCanExecuteChanged kvp.Value)

    let buildProps =
        let toCommand (exec, canExec) =
            let execute = Action (fun p -> exec p model |> dispatch)
            let canExecute = fun p -> canExec p model
            Xamarin.Forms.Command (execute, canExecute)

        let rec convert = 
            List.map (fun (name, binding) ->
                match binding with
                | Bind getter -> name, Get getter
                | BindTwoWay (getter, setter) -> name, GetSet (getter, setter)
                | BindCmd (exec, canExec) -> name, Cmd <| toCommand (exec, canExec)
            )
    
        propMap |> convert |> List.iter props.Add

    do buildProps

    interface ComponentModel.INotifyPropertyChanged with
        [<CLIEvent>]
        member __.PropertyChanged = propertyChanged.Publish

    member __.UpdateModel other =
        //console.log <| sprintf "UpdateModel %A" (props.Keys |> Seq.toArray)
        let propDiff name =
            function
            | Get getter | GetSet (getter, _) ->
                if getter model <> getter other then Some name else None
            | _ -> None

        let diffs = 
            props
            |> Seq.choose (fun kvp -> propDiff kvp.Key kvp.Value)
            |> Seq.toList
        
        model <- other
        notify diffs

    member __.Item 
        with get (name : string) =
            //console.log <| sprintf "Get item %s" name
            if props.ContainsKey name 
            then match props.[name] with 
                 | Get getter -> getter model
                 | GetSet (getter, _) -> getter model
                 | Cmd c -> unbox c
            else invalidOp <| sprintf "Prop Binding Not Set: %s" name
        and set (name : string) (value : obj) =
            //console.log <| sprintf "Set item %s" name
            if props.ContainsKey name 
            then match props.[name] with 
                 | GetSet (_, setter) -> setter value model |> dispatch
                 | _ -> invalidOp "Unable to set read-only member"
            else invalidOp <| sprintf "Prop Binding Not Set: %s" name
