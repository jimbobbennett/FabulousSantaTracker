namespace FabulousSantaTracker.Android

open FabulousSantaTracker
open Android.Content
open Xamarin.Forms
open Xamarin.Forms.Maps
open Xamarin.Forms.Platform.Android
open Xamarin.Forms.Maps.Android
open Android.Gms.Maps.Model;

type SantaMapRenderer(context : Context) =
    inherit MapRenderer(context)

    override this.CreateMarker(pin : Pin) =
        (new MarkerOptions()).SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude))
                             .SetTitle(pin.Label)
                             .SetIcon(BitmapDescriptorFactory.FromResource(Resources.Drawable.Santa))

module Export_SantaMapRenderer =
    [<assembly: ExportRenderer(typeof<CustomMap>, typeof<SantaMapRenderer>) >]
    do ()