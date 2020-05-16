using System;
using System.IO;
using Android.Graphics;
using Android.Media;

namespace MidasPhotoPicker.Droid.Services
{
    public class ImageHelper
    {
        public ImageHelper()
        {

        }

        public static string SaveFile(string sCollName, byte[]iImages, string sFileName)
        {
            //var oFileDir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(
            //    Android.OS.Environment.DirectoryPictures),
            //    sCollName);

            //if (!oFileDir.Exists())
            //    oFileDir.Mkdir();

            //var oFile =  Java.IO.File(oFileDir, sFile);


            //File.WriteAllBytes(oFile, iImages);

            var oFileDir = Android.OS.Environment.GetExternalStoragePublicDirectory(
                            Android.OS.Environment.DirectoryPictures).AbsolutePath;

            string oFile = System.IO.Path.Combine(oFileDir.ToString(), sFileName);
            System.IO.File.WriteAllBytes(oFile, iImages);

            return oFile;

        }

        /* 
        public static byte[] RotateImage(string sPath)
        {
            byte[] imgBytes;
            var oOriginalImg = BitmapFactory.DecodeFile(sPath);
            var oRotation = GetRotation(sPath);
            var oWidth = (oOriginalImg.Width * 0.25);
            var oHeight = (oOriginalImg.Height * 0.25);
            var oScaleImg = Bitmap.CreateScaledBitmap(oOriginalImg, (int)oWidth, (int)oHeight, true);

            Bitmap oBitmap = oScaleImg;

            if (oRotation != 0)
            {
                var oMatrix = new Matrix();
                oMatrix.PostRotate(oRotation);
                oBitmap = Bitmap.CreateBitmap(oScaleImg, 0, 0, oScaleImg.Width, oScaleImg.Height, oMatrix, true);

                oScaleImg.Recycle();
                oScaleImg.Dispose();

            }

            using (var oMS = new MemoryStream())
            {
                oBitmap.Compress(Bitmap.CompressFormat.Jpeg, 90, oMS);
                imgBytes = oMS.ToArray();
            }

            oOriginalImg.Recycle();
            oOriginalImg.Dispose();
            oBitmap.Recycle();
            oBitmap.Dispose();

            GC.Collect();

            return imgBytes;


        }
        */

        public static byte[] RotateImage(string path, float scaleFactor, int quality = 90)
        {
            byte[] imageBytes;

            var originalImage = BitmapFactory.DecodeFile(path);
            var rotation = GetRotation(path);
            var width = (originalImage.Width * scaleFactor);
            var height = (originalImage.Height * scaleFactor);
            var scaledImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, true);

            Bitmap rotatedImage = scaledImage;
            if (rotation != 0)
            {
                var matrix = new Matrix();
                matrix.PostRotate(rotation);
                rotatedImage = Bitmap.CreateBitmap(scaledImage, 0, 0, scaledImage.Width, scaledImage.Height, matrix, true);
                scaledImage.Recycle();
                scaledImage.Dispose();
            }

            using (var ms = new MemoryStream())
            {
                rotatedImage.Compress(Bitmap.CompressFormat.Jpeg, quality, ms);
                imageBytes = ms.ToArray();
            }


            originalImage?.Dispose();
            rotatedImage?.Dispose();
            GC.Collect();

            return imageBytes;
        }

        private static int GetRotation(string filePath)
        {
            using (var ei = new ExifInterface(filePath))
            {
                var orientation = (Android.Media.Orientation)ei.GetAttributeInt(ExifInterface.TagOrientation, (int)Android.Media.Orientation.Normal);

                switch (orientation)
                {
                    case Android.Media.Orientation.Rotate90:
                        return 90;
                    case Android.Media.Orientation.Rotate180:
                        return 180;
                    case Android.Media.Orientation.Rotate270:
                        return 270;
                    default:
                        return 0;
                }
            }
        }


    }
}
