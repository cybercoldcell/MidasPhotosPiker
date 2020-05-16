using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.IO;
using System.Threading.Tasks;
using MidasPhotoPicker.Droid.Services;
using System.Collections.Generic;
using Android.Database;
using Android.Provider;
using Xamarin.Forms;
using Plugin.CurrentActivity;
using Xamarin.Essentials;
using FFImageLoading.Forms.Platform;
using DLToolkit.Forms.Controls;

namespace MidasPhotoPicker.Droid
{
    [Activity(Label = "MidasPhotoPicker", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

       
        protected override void OnCreate(Bundle savedInstanceState)
        {
           
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            CrossCurrentActivity.Current.Activity = this;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);

     
            LoadApplication(new App(PhotoPickerService.Instance));
            
        }
        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

      
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            PhotoPickerService.Instance.OnActivityResult(requestCode, resultCode, data);

        }

    }
}