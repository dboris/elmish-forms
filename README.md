# Elmish Xamarin Forms

## Usage:

```F#
open Elmish
open Elmish.Forms

type Model =
  { Count : int
    Step : int }

type Msg =
    | Increment
    | Decrement
    | Reset
    | SetStep of int

type CounterApp () =
    inherit Xamarin.Forms.Application ()

    let init () = { Count = 0; Step = 1 }

    let update msg model =
        match msg with
        | Increment -> { model with Count = model.Count + model.Step }
        | Decrement -> { model with Count = model.Count - model.Step }
        | Reset -> init ()
        | SetStep n -> { model with Step = n }

    let view _ _ =
        [ "CounterValue" |> Binding.oneWay (fun m -> m.Count)
          "IncrementCommand" |> Binding.cmd (fun _ _ -> Increment)
          "DecrementCommand" |> Binding.cmd (fun _ _ -> Decrement)
          "ResetCommand" |> Binding.cmdIf (fun _ _ -> Reset) (fun _ m -> m <> init ())
          "StepValue" |> Binding.twoWay (fun m -> double m.Step) (fun v m -> v |> int |> SetStep) ]

    let page = Samples.CounterPage ()

    do
        Program.mkSimple init update view
        |> Program.withConsoleTrace
        |> Program.runPage page

        base.MainPage <- page
```

The page XAML:
```xml
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Samples.CounterPage">
    <ContentPage.Content>
        <StackLayout Padding="20" VerticalOptions="CenterAndExpand">
            <Label Text="{Binding Path=[CounterValue], StringFormat='{0}'}" HorizontalOptions="Center" />
            <Button Text="Increment" Command="{Binding Path=[IncrementCommand]}" />
            <Button Text="Decrement" Command="{Binding Path=[DecrementCommand]}" />
            <Button Text="Reset" Command="{Binding Path=[ResetCommand]}" />
            <Slider Maximum="10" Minimum="1" Value="{Binding Path=[StepValue]}" />
            <Label Text="{Binding Path=[StepValue], StringFormat='Step size: {0}'}" HorizontalOptions="Center" />
        </StackLayout>
    </ContentPage.Content>
</ContentPage>
```

See [samples project](https://github.com/dboris/elmish-forms-samples).

Credit goes to the creators of [Elmish.WPF](https://github.com/Prolucid/Elmish.WPF), [Fable-Elmish](https://github.com/fable-elmish/elmish), and [Elm](https://github.com/elm-lang).
