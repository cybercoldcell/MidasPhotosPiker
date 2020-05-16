using System;
using System.ComponentModel;
using MidasPhotoPicker.Interfaces;
using System.Collections;
using System.Collections.ObjectModel;
using MidasPhotoPicker.Models;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace MidasPhotoPicker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        IMediaService _mediaService;
        public ICommand SelectImagesCommand { get; set; }
        public ICommand SelectVideosCommand { get; set; }


        public ObservableCollection<MediaModel> oModel { get; set; }



        public MainViewModel(IMediaService mediaService)
        {
            _mediaService = mediaService;

            SelectImagesCommand = new Command(async (obj) =>
            {
                var hasPermission = await CheckPermissionsAsync();
                if (hasPermission)
                {
                    oModel = new ObservableCollection<MediaModel>();
                    await _mediaService.OnPhotosPick();
                }
            });

            SelectVideosCommand = new Command(async (obj) =>
            {
                var hasPermission = await CheckPermissionsAsync();
                if (hasPermission)
                {

                    oModel = new ObservableCollection<MediaModel>();

                    await _mediaService.OnVideosPick();

                }
            });

            _mediaService.OnMediaPicked += (s, x) => {
                Device.BeginInvokeOnMainThread(() => {
                    oModel.Add(x);
                });

            };

        }

        async Task<bool> CheckPermissionsAsync()
        {
            var bResult = false;
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Plugin.Permissions.Abstractions.Permission.Storage))
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Need Storage permission to access to your photos.", "Ok");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Plugin.Permissions.Abstractions.Permission.Storage });
                    status = results[Plugin.Permissions.Abstractions.Permission.Storage];
                }

                if (status == PermissionStatus.Granted)
                {
                    bResult = true;

                }
                else if (status != PermissionStatus.Unknown)
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "Permission Denied, try again.", "Ok");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await App.Current.MainPage.DisplayAlert("Alert", "Error, try again.", "Ok");
            }

            return bResult;

        }

    }
}
