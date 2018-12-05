namespace FabulousSantaTracker

open System
open System.Timers
open Fabulous.Core
open Fabulous.DynamicViews
open Xamarin.Forms
open Xamarin.Forms.Maps

open TrackingData

module App = 
    type Model = 
        {
            CurrentDestination : Destination
        }

    type Msg = 
        | TimerTick

    let currentDestination () =
        let current = TrackingData.AllDestinations |> Array.tryFindBack (fun i -> i.ArrivalDateTime < DateTime.UtcNow)
        match current with
        | Some d -> d
        | None -> TrackingData.AllDestinations |> Array.item 0

    let init () = { CurrentDestination = currentDestination() }, Cmd.none

    let update msg model =
        match msg with
        | TimerTick -> { model with CurrentDestination = currentDestination() }, Cmd.none

    let inline stringf format (x : ^a) = 
        (^a : (member ToString : string -> string) (x, format))

    let view (model: Model) dispatch =
        View.NavigationPage(
            pages = [
                View.ContentPage(
                    title = "🎅 Tracker",
                    content = View.Grid(
                        padding = 0.0,
                        rowSpacing = 0.,
                        rowdefs = ["auto"; "auto"; "*"],
                        children =[
                            View.StackLayout(
                                orientation = StackOrientation.Horizontal,
                                spacing = 0.,
                                children = [
                                    View.Label(
                                        text = model.CurrentDestination.City,
                                        fontAttributes = FontAttributes.Bold,
                                        verticalOptions = LayoutOptions.End,
                                        margin = new Thickness(5.,0.)
                                    ).GridRow(1)
                                    View.Label(
                                        text = model.CurrentDestination.Region,
                                        fontSize = "Small",
                                        verticalOptions = LayoutOptions.End
                                    )
                                ]
                            ).GridRow(0)
                            View.StackLayout(
                                orientation = StackOrientation.Horizontal,
                                spacing = 0.,
                                children = [
                                    View.Label(
                                        text = "Presents delivered:",
                                        fontAttributes = FontAttributes.Bold,
                                        verticalOptions = LayoutOptions.End,
                                        margin = new Thickness(5.,0.)
                                    ).GridRow(1)
                                    View.Label(
                                        text = (model.CurrentDestination.PresentsDelivered |> stringf "N0"),
                                        fontSize = "Small",
                                        verticalOptions = LayoutOptions.End
                                    )
                                ]
                            ).GridRow(1)
                            View.SantaMap(
                                hasZoomEnabled = true,
                                hasScrollEnabled = true,
                                requestedRegion = MapSpan.FromCenterAndRadius(model.CurrentDestination.Position, Distance.FromKilometers(1000.0)),
                                pins = [ 
                                    View.Pin(
                                        model.CurrentDestination.Position,
                                        label = "Santa",
                                        pinType = PinType.Place
                                    )
                                ]
                            ).GridRow(2)
                        ]
                    )
                )
            ]
        )

    let timerTick dispatch =
        let timer = new Timer(TimeSpan.FromSeconds(10.).TotalMilliseconds)
        timer.Elapsed.Subscribe (fun _ -> dispatch TimerTick) |> ignore
        timer.Enabled <- true
        timer.Start()

    let program = Program.mkProgram init update view

type App () as app = 
    inherit Application ()

    let runner = 
        App.program
        |> Program.withSubscription (fun _ -> Cmd.ofSub App.timerTick)
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> Program.runWithDynamicView app

#if DEBUG
    do runner.EnableLiveUpdate()
#endif    



