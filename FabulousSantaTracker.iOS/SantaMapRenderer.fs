namespace FabulousSantaTracker.iOS

open FabulousSantaTracker
open ObjCRuntime
open Xamarin.Forms
open Xamarin.Forms.Maps.iOS
open MapKit
open UIKit

type SantaMapRenderer() =
    inherit MapRenderer()

    let createSantaPin annotation =
        let santaPin = new MKAnnotationView(annotation, "SantaPin")
        santaPin.Image <- UIImage.FromFile("Santa.png")
        santaPin

    override this.GetViewForAnnotation(mapView : MKMapView, annotation : IMKAnnotation) =
        let baseAnnotation = base.GetViewForAnnotation(mapView, annotation)
        if (baseAnnotation = null) then
            null
        else
            let annotationView = mapView.DequeueReusableAnnotation("SantaPin")
            if (annotationView <> null) then annotationView
            else createSantaPin annotation


module Export_SantaMapRenderer =
    [<assembly: ExportRenderer(typeof<CustomMap>, typeof<SantaMapRenderer>) >]
    do ()