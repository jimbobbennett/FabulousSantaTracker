namespace FabulousSantaTracker

type SantaMap() =
    inherit Xamarin.Forms.Maps.Map()

[<AutoOpen>]
module MapsExtension = 

    open Fabulous.DynamicViews
    open Xamarin.Forms.Maps
    
    type Fabulous.DynamicViews.View with
        /// Describes a Map in the view
        static member inline SantaMap(?pins: seq<ViewElement>, ?isShowingUser: bool, ?mapType: MapType, ?hasScrollEnabled: bool,
                                          ?hasZoomEnabled: bool, ?requestedRegion: MapSpan,
                                          // inherited attributes common to all views
                                          ?horizontalOptions, ?verticalOptions, ?margin, ?gestureRecognizers, ?anchorX, ?anchorY, ?backgroundColor,
                                          ?heightRequest, ?inputTransparent, ?isEnabled, ?isVisible, ?minimumHeightRequest, ?minimumWidthRequest,
                                          ?opacity, ?rotation, ?rotationX, ?rotationY, ?scale, ?style, ?translationX, ?translationY, ?widthRequest,
                                          ?resources, ?styles, ?styleSheets, ?classId, ?styleId, ?automationId, ?created, ?styleClass) =

            // Count the number of additional attributes
            let attribCount = 0
            let attribCount = match pins with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match hasScrollEnabled with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match isShowingUser with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match mapType with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match hasZoomEnabled with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match requestedRegion with Some _ -> attribCount + 1 | None -> attribCount

            // Count and populate the inherited attributes
            let attribs = 
                View.BuildView(attribCount, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions, 
                              ?margin=margin, ?gestureRecognizers=gestureRecognizers, ?anchorX=anchorX, ?anchorY=anchorY, 
                              ?backgroundColor=backgroundColor, ?heightRequest=heightRequest, ?inputTransparent=inputTransparent, 
                              ?isEnabled=isEnabled, ?isVisible=isVisible, ?minimumHeightRequest=minimumHeightRequest,
                              ?minimumWidthRequest=minimumWidthRequest, ?opacity=opacity, ?rotation=rotation, 
                              ?rotationX=rotationX, ?rotationY=rotationY, ?scale=scale, ?style=style, 
                              ?translationX=translationX, ?translationY=translationY, ?widthRequest=widthRequest, 
                              ?resources=resources, ?styles=styles, ?styleSheets=styleSheets, ?classId=classId, ?styleId=styleId,
                              ?automationId=automationId, ?created=created, ?styleClass=styleClass)

            // Add our own attributes. They must have unique names which must match the names below.
            match pins with None -> () | Some v -> attribs.Add(MapPinsAttribKey, v) 
            match hasScrollEnabled with None -> () | Some v -> attribs.Add(MapHasScrollEnabledAttribKey, v) 
            match isShowingUser with None -> () | Some v -> attribs.Add(MapIsShowingUserAttribKey, v) 
            match mapType with None -> () | Some v -> attribs.Add(MapTypeAttribKey, v) 
            match hasZoomEnabled with None -> () | Some v -> attribs.Add(MapHasZoomEnabledAttribKey, v) 
            match requestedRegion with None -> () | Some v -> attribs.Add(MapRequestingRegionAttribKey, v) 

            // The update method
            let update (prevOpt: ViewElement voption) (source: ViewElement) (target: Map) = 
                View.UpdateView (prevOpt, source, target)
                source.UpdatePrimitive(prevOpt, target, MapHasScrollEnabledAttribKey, (fun target v -> target.HasScrollEnabled <- v))
                source.UpdatePrimitive(prevOpt, target, MapHasZoomEnabledAttribKey, (fun target v -> target.HasZoomEnabled <- v))
                source.UpdatePrimitive(prevOpt, target, MapIsShowingUserAttribKey, (fun target v -> target.IsShowingUser <- v))
                source.UpdatePrimitive(prevOpt, target, MapTypeAttribKey, (fun target v -> target.MapType <- v))
                source.UpdateElementCollection(prevOpt, MapPinsAttribKey, target.Pins)
                source.UpdatePrimitive(prevOpt, target, MapRequestingRegionAttribKey, (fun target v -> target.MoveToRegion(v)))

            // The element
            ViewElement.Create<SantaMap>(SantaMap, update, attribs)