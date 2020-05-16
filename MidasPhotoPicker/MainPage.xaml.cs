using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;
using MidasPhotoPicker.Interfaces;


namespace MidasPhotoPicker
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        //List<string> oImages = new List<string>();

        public MainPage()
        {
            InitializeComponent();

            //Clear function of images.

        }

        /*
        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            
            (sender as Button).IsEnabled = false;

            Stream oStream = await DependencyService.Get<IMediaService>().SelectImageStreamAsync();

            if (oStream != null)
            {
                ImageControl.Source = ImageSource.FromStream(() => oStream);
            }

            (sender as Button).IsEnabled = true;
            

        }
        */

        /* 
        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<App, List<string>>((App)Xamarin.Forms.Application.Current,"SelectedImages", (s, xList) =>
                {
                    listItems.FlowItemsSource = xList;
                    oImages = xList;
                });

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<App, List<string>>(this, "SelectedImages");

        }

        void Handle_Clicked(System.Object sender, System.EventArgs e)
        {
            DependencyService.Get<IMediaService>().SelectPhotos();
        }

        */

        
    }
}
