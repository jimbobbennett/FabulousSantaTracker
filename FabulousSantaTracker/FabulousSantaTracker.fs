namespace FabulousSantaTracker

open System
open System.Timers
open Fabulous.Core
open Fabulous.DynamicViews
open Xamarin.Forms
open Xamarin.Forms.Maps
open Plugin.Permissions
open Plugin.Permissions.Abstractions

open TrackingData

module App = 
    type Model = 
        {
            Destinations : Destinations
            CurrentDestination : Destination
            CurrentTime : DateTime
            ShowCurrentUser : bool
        }

    type Msg = 
        | TimerTick of DateTime
        | SetLocationPermission of bool

    let requestPermissions = 
        async {
            try
                let! status = CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationWhenInUse) |> Async.AwaitTask
                if status = PermissionStatus.Granted then
                    return (SetLocationPermission)true
                else
                    let! status = CrossPermissions.Current.RequestPermissionsAsync([| Permission.LocationWhenInUse |]) |> Async.AwaitTask
                    let granted = status.[Permission.LocationWhenInUse] = PermissionStatus.Granted || status.[Permission.LocationWhenInUse] = PermissionStatus.Unknown
                    return (SetLocationPermission)granted
            with exn ->
                return (SetLocationPermission)false
        }

    let currentDestination (destinations : Destinations) dateTime =
        let first = destinations |> Array.item 0
        if (dateTime < first.ArrivalDateTime) then
            first
        else
           destinations |> Array.findBack (fun i -> i.ArrivalDateTime < dateTime)

    let initModel = 
        { 
            Destinations = AllDestinations
            CurrentDestination = (currentDestination AllDestinations DateTime.UtcNow)
            CurrentTime = DateTime.UtcNow
            ShowCurrentUser = false
        }

    let init () = initModel, (requestPermissions |> Cmd.ofAsyncMsg)

    let update msg model =
        match msg with
        | TimerTick d -> { model with CurrentDestination = (currentDestination model.Destinations d); CurrentTime = d }, Cmd.none
        | SetLocationPermission b -> { model with ShowCurrentUser = b }, Cmd.none

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
                            View.CustomMap(
                                hasZoomEnabled = true,
                                hasScrollEnabled = true,
                                isShowingUser = model.ShowCurrentUser,
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
        timer.Elapsed.Subscribe (fun _ -> dispatch (TimerTick System.DateTime.Now)) |> ignore
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



