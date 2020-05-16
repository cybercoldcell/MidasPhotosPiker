using System;
using MidasPhotoPicker.Models;
using System.IO;


namespace MidasPhotoPicker.Services
{
    public class FileService
    {
        public static string GetUniquePath(MediaFileType oType, string path, string sName)
        {
            string ext = Path.GetExtension(sName);
            if (ext == string.Empty)
                ext = ((oType == MediaFileType.Image) ? ".jpg" : ".mp4");

            sName = Path.GetFileNameWithoutExtension(sName);

            string nname = sName + ext;
            int i = 1;
            while (File.Exists(Path.Combine(path, nname)))
                nname = sName + "_" + (i++) + ext;

            return Path.Combine(path, nname);
        }


        public static string GetOutputPath(MediaFileType type, string path, string name)
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), path);
            Directory.CreateDirectory(path);

            if (string.IsNullOrWhiteSpace(name))
            {
                string timestamp = DateTime.Now.ToString("yyyMMdd_HHmmss");
                if (type == MediaFileType.Image)
                    name = "IMG_" + timestamp + ".jpg";
                else
                    name = "VID_" + timestamp + ".mp4";
            }

            return Path.Combine(path, GetUniquePath(type, path, name));
        }
    }
}
