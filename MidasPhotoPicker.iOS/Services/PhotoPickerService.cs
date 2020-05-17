using System;
using System.IO;
using System.Threading.Tasks;
using MidasPhotoPicker.Interfaces;
using MidasPhotoPicker.iOS.Services;
using Foundation;
using UIKit;
using Xamarin.Forms;
using MidasPhotoPicker.Models;
using System.Collections.Generic;
using GMImagePicker;
using Photos;
using System.Linq;
using MidasPhotoPicker.Services;
using System.Diagnostics;
using System.Drawing;
using AVFoundation;

[assembly: Dependency(typeof(PhotoPickerService))]
namespace MidasPhotoPicker.iOS.Services
{
    public class PhotoPickerService : IMediaService
    {
        /* 
        TaskCompletionSource<Stream> _taskCompletionSource;
        UIImagePickerController _imagePickerController;

        public Task<Stream> SelectImageStreamAsync()
        {
            // Create and define image types.
            _imagePickerController = new UIImagePickerController
            {
                SourceType = UIImagePickerControllerSourceType.PhotoLibrary
                , MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary)

            };

            // Set event
            _imagePickerController.FinishedPickingMedia += OnImagePickerFinishedPickingMedia;
            _imagePickerController.Canceled += OnImagePickerCanceled;

            // Present view controller
            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            var viewController = window.RootViewController;
            viewController.PresentModalViewController(_imagePickerController, true);


            _taskCompletionSource = new TaskCompletionSource<Stream>();
            return _taskCompletionSource.Task;

        }

  
        private void OnImagePickerFinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            UIImage image = e.EditedImage ?? e.OriginalImage;

            if (image != null)
            {
                // Convert UIImage to .NET Stream object
                NSData oData;
                if (e.ReferenceUrl.PathExtension.Equals("PNG") || e.ReferenceUrl.PathExtension.Equals("JPG"))
                {
                    oData = image.AsPNG();
                }
                else
                {
                    oData = image.AsJPEG(1);
                }

                Stream oStream = oData.AsStream();

                UnregisterEventHandlers();

                _taskCompletionSource.SetResult(oStream);

            }
            else
            {
                UnregisterEventHandlers();
                _taskCompletionSource.SetResult(null);
            }

            _imagePickerController.DismissModalViewController(true);
        }


        private void OnImagePickerCanceled(object sender, EventArgs e)
        {
            UnregisterEventHandlers();
            _taskCompletionSource.SetResult(null);
            _imagePickerController.DismissModalViewController(true);

        }

        private void UnregisterEventHandlers()
        {
            _imagePickerController.FinishedPickingMedia -= OnImagePickerFinishedPickingMedia;
            _imagePickerController.Canceled -= OnImagePickerCanceled;
        }

       */

        //public void SelectPhotos()
        //{
        //    throw new NotImplementedException();
        //}

        const string TempDir = "TempMedia";
        GMImagePickerController GMPicker;
        TaskCompletionSource<IList<MediaModel>> _taskCompletionSource;


        public event EventHandler<MediaModel> OnMediaPicked;
        public event EventHandler<IList<MediaModel>> OnMediaPickedCompleted;

        public void ClearFile()
        {
            var docDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
              TempDir);

            if (Directory.Exists(docDirectory))
            {
                Directory.Delete(docDirectory);
            }
        }

        public async Task<IList<MediaModel>> OnPhotosPick()
        {
            return await PickMediaAsync("Select Images", PHAssetMediaType.Image);
        }

        public async Task<IList<MediaModel>> OnVideosPick()
        {
            return await PickMediaAsync("Select videos", PHAssetMediaType.Video);
        }

        async Task<IList<MediaModel>> PickMediaAsync(string sTitle, PHAssetMediaType oType)
        {
            _taskCompletionSource = new TaskCompletionSource<IList<MediaModel>>();

            GMPicker = new GMImagePickerController()
            {
                Title = sTitle,
                MediaTypes = new[] { oType }
            };

            GMPicker.FinishedPickingAssets += FinishedPickingAsync;
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }
            await vc.PresentViewControllerAsync(GMPicker, true);
            var results = await _taskCompletionSource.Task;
            GMPicker.FinishedPickingAssets -= FinishedPickingAsync;
            OnMediaPickedCompleted?.Invoke(this, results);
            return results;

        }

        async void FinishedPickingAsync(object sender, MultiAssetEventArgs args)
        {
            IList<MediaModel> oList = new List<MediaModel>();
            TaskCompletionSource<IList<MediaModel>> taskCompletionSource = new TaskCompletionSource<IList<MediaModel>>();
            var options = new PHImageRequestOptions()
            {
                NetworkAccessAllowed = true
            };
            options.Synchronous = false;
            options.ResizeMode = PHImageRequestOptionsResizeMode.Fast;
            options.DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat;
            bool completed = false;
            for (var i = 0; i < args.Assets.Length; i++)
            {
                var asset = args.Assets[i];
                string fileName = string.Empty;
                if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
                {
                    fileName = PHAssetResource.GetAssetResources(asset).FirstOrDefault().OriginalFilename;
                }
                switch (asset.MediaType)
                {
                    case PHAssetMediaType.Video:
                        PHImageManager.DefaultManager.RequestImageForAsset(asset, new SizeF(150.0f, 150.0f),
                        PHImageContentMode.AspectFill, options, async (img, info) =>
                        {
                            var startIndex = fileName.IndexOf(".", StringComparison.CurrentCulture);
                            string path = "";
                            if (startIndex != -1)
                            {
                                path = FileService.GetOutputPath(MediaFileType.Image, TempDir, $"{fileName.Substring(0, startIndex)}-THUMBNAIL.JPG");
                            }
                            else
                            {
                                path = FileService.GetOutputPath(MediaFileType.Image, TempDir, string.Empty);
                            }
                            if (!File.Exists(path))
                            {
                                img.AsJPEG().Save(path, true);
                            }
                            TaskCompletionSource<string> tvcs = new TaskCompletionSource<string>();
                            var vOptions = new PHVideoRequestOptions();
                            vOptions.NetworkAccessAllowed = true;
                            vOptions.Version = PHVideoRequestOptionsVersion.Original;
                            vOptions.DeliveryMode = PHVideoRequestOptionsDeliveryMode.FastFormat;
                            PHImageManager.DefaultManager.RequestAvAsset(asset, vOptions, (avAsset, audioMix, vInfo) =>
                            {
                                var vPath = FileService.GetOutputPath(MediaFileType.Video, TempDir, fileName);
                                if (!File.Exists(vPath))
                                {
                                    AVAssetExportSession exportSession = new AVAssetExportSession(avAsset, AVAssetExportSession.PresetHighestQuality);
                                    exportSession.OutputUrl = NSUrl.FromFilename(vPath);
                                    exportSession.OutputFileType = AVFileType.QuickTimeMovie;
                                    exportSession.ExportAsynchronously(() =>
                                    {
                                        Console.WriteLine(exportSession.Status);
                                        tvcs.TrySetResult(vPath);
                                        //exportSession.Dispose();
                                    });
                                }
                            });

                            var videoUrl = await tvcs.Task;
                            var oModel = new MediaModel
                            {
                                MediaType = MediaFileType.Video,
                                FilePath = videoUrl,
                                PreviewFile = path
                            };
                            oList.Add(oModel);
                            OnMediaPicked?.Invoke(this, oModel);
                            if (args.Assets.Length == oList.Count && !completed)
                            {
                                completed = true;
                                taskCompletionSource.TrySetResult(oList);
                            }
                        });
                        break;
                    default:
                        PHImageManager.DefaultManager.RequestImageData(asset, options, (data, dataUti, orientation, info) =>
                        {
                            string path = FileService.GetOutputPath(MediaFileType.Image, TempDir, fileName);
                            if (!File.Exists(path))
                            {
                                Debug.WriteLine(dataUti);
                                var imageData = data;
            
                                imageData?.Save(path, true);
                            }
                            var oModel = new MediaModel()
                            {
                                MediaType = MediaFileType.Image,
                                FilePath = path,
                                PreviewFile = path
                            };
                            oList.Add(oModel);
                            OnMediaPicked?.Invoke(this, oModel);
                            if (args.Assets.Length == oList.Count && !completed)
                            {
                                completed = true;
                                taskCompletionSource.TrySetResult(oList);
                            }
                        });
                        break;
                }
            }

            _taskCompletionSource?.TrySetResult(await taskCompletionSource.Task);

        }

    }
}
