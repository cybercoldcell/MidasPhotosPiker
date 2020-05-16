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

        //internal static MainActivity Instance { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Instance = this; //added.

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

        //*/

        /// Field, property, and method for Picture Picker
        //public static readonly int iPickImageId = 1000;
        //public TaskCompletionSource<Stream> ImagePickTaskCompletionSource { get; set; }

        //protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        //{
        //    base.OnActivityResult(requestCode, resultCode, data);

        //    if (requestCode == iPickImageId)
        //    {
        //        Android.Net.Uri uri = data.Data;
        //        Stream oStream = ContentResolver.OpenInputStream(uri);

        //        ImagePickTaskCompletionSource.SetResult(oStream);

        //    }
        //    else
        //    {
        //        ImagePickTaskCompletionSource.SetResult(null);
        //    }

        //}


        /* 
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == PhotoPickerService.IMGS_SELECTED && resultCode == Result.Ok)
            {
                List<string> oImages = new List<string>();

                if (data != null)
                {
                    ClipData oClipData = data.ClipData;

                    if (oClipData != null)
                    {
                        for (int i = 0; i < oClipData.ItemCount; i++)
                        {
                            ClipData.Item oItem = oClipData.GetItemAt(i);
                            Android.Net.Uri oUri = oItem.Uri;

                            var oPath = GetPathUri(oUri);

                            if (oPath != null)
                            {
                                //Rotate image
                                var oRotImage = ImageHelper.RotateImage(oPath);
                                var oNewPath = ImageHelper.SaveFile("TmpPhotos", oRotImage, "IMG_" + System.DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                                oImages.Add(oNewPath);
                            }


                        }

                    }
                    else
                    {
                        Android.Net.Uri oUri= data.Data;

                        var oPath = GetPathUri(oUri);

                        if(oPath != null)
                        {
                            //Rotate image
                            var oRotImage = ImageHelper.RotateImage(oPath);
                            var oNewPath = ImageHelper.SaveFile("TmpPhotos", oRotImage, "IMG_" + System.DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                            oImages.Add(oNewPath);
                        }

                    }

                    MessagingCenter.Send<App, List<string>>((App)Xamarin.Forms.Application.Current,"SelectedImages",oImages);

                }


            }

        }

        //Getting the image uri path.
        [Obsolete]
        private string GetPathUri(Android.Net.Uri oUri)
        {
            ICursor oImgCursor = null;
            string sPathUri = null;

            try
            {

                oImgCursor = ContentResolver.Query(oUri, null, null, null);
                oImgCursor.MoveToFirst();

                int iIndex = oImgCursor.GetColumnIndex(MediaStore.Images.ImageColumns.Data);

                if (iIndex != -1)
                {
                    //old devices;
                    sPathUri = oImgCursor.GetString(iIndex);
                }
                else
                {
                    //new devices;
                    oImgCursor = null;

                    var oDocId = DocumentsContract.GetDocumentId(oUri);
                    var oId = oDocId.Split(":")[1];
                    var oSelect = MediaStore.Images.ImageColumns.Id + "=?";
                    var oProjections = new string[] { MediaStore.Images.ImageColumns.Data };
                    //Internal storage
                    oImgCursor = ContentResolver.Query(MediaStore.Images.Media.InternalContentUri, oProjections,
                        oSelect, new string[] { oId }, null);

                    if (oImgCursor.Count == 0)
                    {
                        //not found in internal storage
                        oImgCursor = ContentResolver.Query(MediaStore.Images.Media.ExternalContentUri, oProjections,
                            oSelect, new string[] { oId }, null);
                        var oColData = oImgCursor.GetColumnIndexOrThrow(MediaStore.Images.ImageColumns.Data);
                        oImgCursor.MoveToFirst();

                        sPathUri = oImgCursor.GetString(oColData);

                    }

                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(Xamarin.Forms.Forms.Context, "Error Uri: " + ex.Message.ToString(), ToastLength.Long).Show();
            }

            return sPathUri;
        }

        */


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            PhotoPickerService.Instance.OnActivityResult(requestCode, resultCode, data);

        }

    }
}