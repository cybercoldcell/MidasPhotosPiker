using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using MidasPhotoPicker.Models;


namespace MidasPhotoPicker.Interfaces
{
    public interface IMediaService
    {
        //Task<Stream> SelectImageStreamAsync();

        //void SelectPhotos();

        event EventHandler<MediaModel> OnMediaPicked;
        event EventHandler<IList<MediaModel>> OnMediaPickedCompleted;

        Task<IList<MediaModel>> OnPhotosPick();
        Task<IList<MediaModel>> OnVideosPick();
        void ClearFile();

    }
}
