using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DLToolkit.Forms.Controls;
using MidasPhotoPicker.Interfaces;
using MidasPhotoPicker.ViewModels;


namespace MidasPhotoPicker
{
    public partial class App : Application
    {
        public App(IMediaService mediaService)
        {
            InitializeComponent();
            FlowListView.Init();

            MainPage = new MainPage()
            {
                BindingContext = new MainViewModel(mediaService)
            };
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
