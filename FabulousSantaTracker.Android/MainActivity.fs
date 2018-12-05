namespace FabulousSantaTracker.Android

open System

open Android
open Android.App
open Android.Content.PM
open Android.Runtime
open Android.OS
open Xamarin
open Xamarin.Essentials
open Xamarin.Forms
open Xamarin.Forms.Platform.Android
open Xamarin.Forms.Maps
open Plugin.CurrentActivity

[<Activity (Label = "Santa Tracker", Icon = "@mipmap/icon", Theme = "@style/MainTheme.Launcher", MainLauncher = true, ConfigurationChanges = (ConfigChanges.ScreenSize ||| ConfigChanges.Orientation))>]
type MainActivity() =
    inherit FormsAppCompatActivity()

    override this.OnCreate (bundle: Bundle) =
        FormsAppCompatActivity.TabLayoutResource <- Resources.Layout.Tabbar
        FormsAppCompatActivity.ToolbarResource <- Resources.Layout.Toolbar
        this.SetTheme(Resources.Style.MainTheme)
        base.OnCreate (bundle)

        Platform.Init(this, bundle)
        Forms.Init (this, bundle)
        FormsMaps.Init(this, bundle)
        CrossCurrentActivity.Current.Init(this, bundle)

        let appcore  = new FabulousSantaTracker.App()
        this.LoadApplication (appcore)
                                 
    override this.OnRequestPermissionsResult(requestCode: int, permissions: string[], [<GeneratedEnum>] grantResults: Android.Content.PM.Permission[]) =
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults)
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults)
