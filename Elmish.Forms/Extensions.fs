module Elmish.Program

    let runPage (page : Xamarin.Forms.Page) (program : Program<unit, 'model, 'msg, Elmish.Forms.ViewBindings<'model, 'msg>>) = 

        let mutable lastModel = None

        let setState model dispatch = 
            match lastModel with
            | None -> 
                let mapping = program.view model dispatch
                let vm = Elmish.Forms.ViewModelBase<'model, 'msg> (model, dispatch, mapping)
                page.BindingContext <- vm
                //console.log <| sprintf "VM mapping: %A" mapping
                lastModel <- Some vm
            | Some vm ->
                vm.UpdateModel model
                      
        // Start Elmish dispatch loop  
        { program with setState = setState } 
        |> Elmish.Program.run
