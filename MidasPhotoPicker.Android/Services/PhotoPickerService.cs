using System;
using System.IO;
using System.Threading.Tasks;
using MidasPhotoPicker.Interfaces;
using MidasPhotoPicker.Droid.Services;
using Xamarin.Forms;
using Android.Content;
using Android.Widget;
using Android.App;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using MidasPhotoPicker.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Android.Database;
using Plugin.CurrentActivity;
using Android.Provider;
using MidasPhotoPicker.Services;
using Android.Media;
using Android.Graphics;

[assembly: Dependency(typeof(PhotoPickerService))]
namespace MidasPhotoPicker.Droid.Services
{
    public class PhotoPickerService : Java.Lang.Object, IMediaService
    {
        /*
        public Task<Stream> SelectImageStreamAsync()
        {
            // Define the internet for getting images.
            Intent oIntent = new Intent();
            oIntent.SetType("image/*");
            oIntent.SetAction(Intent.ActionGetContent);

            // Start the picture-picker activity (resumes in MainActivity.cs)
            MainActivity.Instance.StartActivityForResult(
                    Intent.CreateChooser(oIntent, "Select image"),
                    MainActivity.iPickImageId
                );

            // Save the TaskCompletionSource object as a MainActivity property
            MainActivity.Instance.ImagePickTaskCompletionSource = new TaskCompletionSource<Stream>();

            return MainActivity.Instance.ImagePickTaskCompletionSource.Task;

        }
        */

        //public static int IMGS_SELECTED = 200;



        //[Obsolete]
        //public async void SelectPhotos()
        //{

        //    try
        //    {
        //        var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);

        //        if (status != PermissionStatus.Granted)
        //        {
        //            if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Plugin.Permissions.Abstractions.Permission.Storage))
        //            {
        //                Toast.MakeText(Xamarin.Forms.Forms.Context, "Need permission to access your photos.", ToastLength.Short).Show();

        //            }

        //            var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Plugin.Permissions.Abstractions.Permission.Storage });
        //            status = results[Plugin.Permissions.Abstractions.Permission.Storage];
        //        }

        //        if (status == PermissionStatus.Granted)
        //        {
        //            Toast.MakeText(Xamarin.Forms.Forms.Context, "Maximam 4 photos.", ToastLength.Long);
        //            var oIntent = new Intent(Intent.ActionPick);
        //            oIntent.SetType("image/*");
        //            oIntent.PutExtra(Intent.ExtraAllowMultiple, true);
        //            oIntent.SetAction(Intent.ActionGetContent);

        //            ((Activity)Forms.Context).StartActivityForResult(Intent.CreateChooser(oIntent, "Select photos"), IMGS_SELECTED);
        //            //MainActivity.Instance.StartActivityForResult(Intent.CreateChooser(oIntent, "Select photos"), IMGS_SELECTED);
        //        }
        //        else if (status != PermissionStatus.Unknown)
        //        {
        //            Toast.MakeText(Xamarin.Forms.Forms.Context, "Permission Denied. Please try again.", ToastLength.Long).Show();
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        Toast.MakeText(Xamarin.Forms.Forms.Context, "Error: " + ex.Message.ToString(), ToastLength.Long).Show();
        //    }

        //}

        public static PhotoPickerService Instance = new PhotoPickerService();
        int SOURCE_PICK_CODE = 9793;
        const string TempDir = "TempMedia";

        TaskCompletionSource<IList<MediaModel>> _taskCompletionSource;

        public event EventHandler<MediaModel> OnMediaPicked;
        public event EventHandler<IList<MediaModel>> OnMediaPickedCompleted;

        public void ClearFile()
        {
            var DocDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), TempDir);

            if (Directory.Exists(DocDir))
            {
                Directory.Delete(DocDir);
            }
        }

        public async Task<IList<MediaModel>> OnPhotosPick()
        {
            return await PickMediaAsync("image/*", "Select images", SOURCE_PICK_CODE);
        }

        public async Task<IList<MediaModel>> OnVideosPick()
        {
            return await PickMediaAsync("","Select videos", SOURCE_PICK_CODE);
        }

        private async Task<IList<MediaModel>> PickMediaAsync(string oType, string sTitle, int iResultCode)
        {

            _taskCompletionSource = new TaskCompletionSource<IList<MediaModel>>();

            var oIntent = new Intent(Intent.ActionPick);
            oIntent.SetType(oType);
            oIntent.PutExtra(Intent.ExtraAllowMultiple, true);
            CrossCurrentActivity.Current.Activity.StartActivityForResult(Intent.CreateChooser(oIntent, sTitle), iResultCode);

            return await _taskCompletionSource.Task;

        }

        [Obsolete]
        public void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            ObservableCollection<MediaModel> oCollection = null;

            if (requestCode == SOURCE_PICK_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    oCollection = new ObservableCollection<MediaModel>();

                    if (data != null)
                    {
                        ClipData oData = data.ClipData;

                        if (oData != null)
                        {
                            for (int i = 0; i < oData.ItemCount; i++)
                            {
                                ClipData.Item oItem = oData.GetItemAt(i);
                                Android.Net.Uri oUri = oItem.Uri;
                                var oModel = CreateMediaModel(oUri);
                                if (oModel != null)
                                {
                                    oCollection.Add(oModel);
                                    OnMediaPicked?.Invoke(this, oModel);
                                }

                            }
                        }
                        else
                        {
                            Android.Net.Uri oUri = data.Data;
                            var oModel = CreateMediaModel(oUri);

                            if (oModel != null)
                            {
                                oCollection.Add(oModel);
                                OnMediaPicked?.Invoke(this, oModel);
                            }
                        }

                        OnMediaPickedCompleted?.Invoke(this, oCollection);

                    }

                }

                _taskCompletionSource?.TrySetResult(oCollection);

            }

        }

        [Obsolete]
        private MediaModel CreateMediaModel(Android.Net.Uri oUri)
        {
            MediaModel oModel = null;
            var oType = CrossCurrentActivity.Current.Activity.ContentResolver.GetType(oUri);

            var oPath = GetPathUri(oUri);
            if (oPath != null)
            {
                string sPath = string.Empty;
                string sThumpnailPath = string.Empty;
                string sFileName = System.IO.Path.GetFileName(oPath);
                string sExt = System.IO.Path.GetExtension(oPath) ?? string.Empty;
                MediaFileType mediaFileType = MediaFileType.Image;
               


                if (oType.StartsWith(Enum.GetName(typeof(MediaFileType), MediaFileType.Image), StringComparison.CurrentCultureIgnoreCase))
                {
                    var fullImage = ImageHelper.RotateImage(oPath,1);

                    sPath = FileService.GetOutputPath(MediaFileType.Image, TempDir, $"{sFileName}{sExt}");
                    File.WriteAllBytes(sPath, fullImage);

                    var oThumpnailImg = ImageHelper.RotateImage(oPath, 0.25f);
                    //sThumpnailPath = FileService.GetOutputPath(MediaFileType.Image, TempDir, $"{sFileName}{sExt}");
                    sThumpnailPath = FileService.GetOutputPath(MediaFileType.Image, TempDir, $"{sFileName}-THUMBNAIL{sExt}");

                    File.WriteAllBytes(sThumpnailPath, oThumpnailImg);

                }
                else if (oType.StartsWith(Enum.GetName(typeof(MediaFileType), MediaFileType.Video), StringComparison.CurrentCultureIgnoreCase))
                {
                    sPath = oPath;
                    var bitmap = ThumbnailUtils.CreateVideoThumbnail(oPath, ThumbnailKind.MiniKind);

                    sThumpnailPath = FileService.GetOutputPath(MediaFileType.Image, TempDir, $"{sFileName}-THUMBNAIL{sExt}");
                    var stream = new FileStream(sThumpnailPath, FileMode.Create);
                    bitmap?.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                    stream.Close();

                    mediaFileType = MediaFileType.Video;
                }

                if (!string.IsNullOrEmpty(sPath) && !string.IsNullOrEmpty(sThumpnailPath))
                {
                    oModel = new MediaModel()
                    {
                        FilePath = sPath,
                        MediaType = mediaFileType,
                        PreviewFile = sThumpnailPath
                    };
                }


            }


            return oModel;
        }

        //Getting the image uri path.
        [Obsolete]
        private string GetPathUri(Android.Net.Uri oUri)
        {
            ICursor oImgCursor = null;
            string sPathUri = null;


            try
            {

                oImgCursor = CrossCurrentActivity.Current.Activity.ContentResolver.Query(oUri, null, null, null);
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
                    var oDoc = oDocId.Split(":");
                    var oId = oDoc[1];
                    var oSelect = MediaStore.Images.ImageColumns.Id + "=?";
                    var oDataConst = MediaStore.Images.ImageColumns.Data;
                    var oProjections = new string[] { oDataConst };
                    var oInternalUri = MediaStore.Images.Media.InternalContentUri;
                    var oExternalUri = MediaStore.Images.Media.ExternalContentUri;


                    if (oDoc[0].Equals("video"))
                    {
                        oSelect = MediaStore.Video.VideoColumns.Id + "=?";
                        oDataConst = MediaStore.Video.VideoColumns.Data;
                        oProjections = new string[] { oDataConst };

                    }

                    oImgCursor = CrossCurrentActivity.Current.Activity.ContentResolver.Query(oInternalUri, oProjections,
                        oSelect, new string[] { oId }, null);

                    if (oImgCursor.Count == 0)
                    {
                        //not found in internal storage
                        oImgCursor = CrossCurrentActivity.Current.Activity.ContentResolver.Query(oExternalUri, oProjections,
                            oSelect, new string[] { oId }, null);
                        var oColData = oImgCursor.GetColumnIndexOrThrow(oDataConst);
                        oImgCursor.MoveToFirst();

                        sPathUri = oImgCursor.GetString(oColData);

                    }

                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(Xamarin.Forms.Forms.Context, "Error Uri: " + ex.Message.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                oImgCursor.Close();
                oImgCursor.Dispose();
            }

            return sPathUri;
        }
    }
}
