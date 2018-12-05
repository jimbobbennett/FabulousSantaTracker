module TrackingData

    open System
    open System.Reflection
    open System.IO
    open Newtonsoft.Json
    open Xamarin.Forms.Maps

    let epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)

    let convertFromEpoch ms =
        let d = epoch.AddMilliseconds (ms)
        d.AddYears(DateTime.UtcNow.Year - d.Year)

    type Location = { Lat : float; Lng : float }

    type Destination = 
        {
            Arrival : float
            PresentsDelivered : int64
            City : string
            Region : string
            Location : Location
        } with
            member this.ArrivalDateTime 
                with get () = convertFromEpoch this.Arrival
            member this.Position
                with get () = new Position(this.Location.Lat, this.Location.Lng)


    type Destinations = array<Destination>

    type SantaData = { Destinations : Destinations }

    let GetResourceString fileName = 
        let assembly = IntrospectionExtensions.GetTypeInfo(typedefof<Destination>).Assembly
        use stream = assembly.GetManifestResourceStream(fileName)
        use reader = new StreamReader (stream)
        reader.ReadToEnd()

    let AllDestinations =
        let santaData = JsonConvert.DeserializeObject<SantaData>(GetResourceString "FabulousSantaTracker.santa_en.json")
        santaData.Destinations
