using System;
namespace MidasPhotoPicker.Models
{
    public enum MediaFileType
    {
        Image,
        Video
    }


    public class MediaModel
    {
    
        public string PreviewFile { get; set; }
        public string FilePath { get; set; }
        public MediaFileType MediaType { get; set; }

    }
}
