using System;
using System.IO;
using System.Threading.Tasks;
using MidasPhotoPicker.Interfaces;
using MidasPhotoPicker.iOS.Services;
using Foundation;
using UIKit;
using Xamarin.Forms;


[assembly: Dependency(typeof(PhotoPickerService))]
namespace MidasPhotoPicker.iOS.Services
{
    public class PhotoPickerService 
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
    }
}
