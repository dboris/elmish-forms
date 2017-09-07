namespace Elmish.Forms


type Getter<'model> = 
    'model -> obj

type Setter<'model, 'msg> = 
    obj -> 'model -> 'msg

type Execute<'model, 'msg> = 
    obj -> 'model -> 'msg

type CanExecute<'model> = 
    obj -> 'model -> bool

type ViewBinding<'model, 'msg> = 
    string * Variable<'model, 'msg>

and ViewBindings<'model, 'msg> = 
    ViewBinding<'model, 'msg> list

and Variable<'model, 'msg> =
    | Bind of Getter<'model>
    | BindTwoWay of Getter<'model> * Setter<'model, 'msg>
    | BindCmd of Execute<'model, 'msg> * CanExecute<'model>

[<RequireQualifiedAccess>]
module Binding =

    let oneWay (getter : 'model -> 'a) p : ViewBinding<'model, 'msg> = 
        p, Bind (getter >> unbox)

    let twoWay (getter : 'model -> 'a) (setter : 'a -> 'model -> 'msg) p : ViewBinding<'model, 'msg> = 
        p, BindTwoWay (getter >> unbox, fun v m -> setter (v :?> 'a) m)

    let cmd exec p : ViewBinding<'model, 'msg> = 
        p, BindCmd (exec, fun _ _ -> true)

    let cmdIf exec canExec p : ViewBinding<'model, 'msg> = 
        p, BindCmd (exec, canExec)
