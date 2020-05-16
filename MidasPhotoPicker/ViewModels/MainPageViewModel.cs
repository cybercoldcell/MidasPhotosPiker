using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using MidasPhotoPicker.Interfaces;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;
using System.Threading.Tasks;
using MidasPhotoPicker.Models;
using System.Windows.Input;


namespace MidasPhotoPicker.ViewModels
{
    public class MainPageViewModel : BindableObject
    {
        private MainPage _mainPage;
        private IMediaService _mediaService;

        [Obsolete]
        public MainPageViewModel(MainPage mainPage, IMediaService mediaService)
        {
            _mainPage = mainPage;
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

        public ICommand SelectImagesCommand { get; set; }
        public ICommand SelectVideosCommand { get; set; }


        private ObservableCollection<MediaModel> _oModel = new ObservableCollection<MediaModel>();
        public ObservableCollection<MediaModel> oModel
        {
            get
            {
                return _oModel;
            }
            set
            {
                if (_oModel != value)
                {
                    _oModel = value;
                    OnPropertyChanged(nameof(oModel));
                }
            }
        }


        [Obsolete]
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
