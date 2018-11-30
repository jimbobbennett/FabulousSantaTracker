module TrackingData

    open System
    open System.Reflection
    open System.IO
    open Newtonsoft.Json
    open Xamarin.Forms.Maps

    let epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)

    let isTestingMode = false

    let mutable offsetForTesting = TimeSpan.FromMilliseconds(0.)

    let calculateOffset (firstItemArrival : DateTime) =
        offsetForTesting <- DateTime.UtcNow.Subtract(firstItemArrival)
        ()

    type Location = { Lat : float; Lng : float }

    type Destination = 
        {
            Id : string
            Arrival : float
            PresentsDelivered : int64
            City : string
            Region : string
            Location : Location
        } with
            member this.ArrivalDateTime 
                with get () = 
                    let arrivalDate = epoch.AddMilliseconds (this.Arrival)
                    let adjustedForThisYear = new DateTime (DateTime.UtcNow.Year, arrivalDate.Month, arrivalDate.Day, arrivalDate.Hour, arrivalDate.Minute, arrivalDate.Second)
                    let withOffset = adjustedForThisYear.Add(offsetForTesting)
                    withOffset
            member this.Position
                with get () =
                    new Position(this.Location.Lat, this.Location.Lng)


    type Destinations = array<Destination>

    type SantaData = { Destinations : Destinations }

    let AllDestinations =
        let assembly = IntrospectionExtensions.GetTypeInfo(typedefof<Destination>).Assembly;
        use stream = assembly.GetManifestResourceStream("FabulousSantaTracker.santa_en.json");
        use reader = new StreamReader (stream)
        let json = reader.ReadToEnd();
        let santaData = JsonConvert.DeserializeObject<SantaData>(json)

        if isTestingMode then
            calculateOffset (santaData.Destinations |> Array.item 1).ArrivalDateTime

        santaData.Destinations
