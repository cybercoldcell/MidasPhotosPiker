using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;
using MidasPhotoPicker.Interfaces;
using MidasPhotoPicker.ViewModels;


namespace MidasPhotoPicker
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        [Obsolete]
        public MainPage(IMediaService mediaService)
        {
            InitializeComponent();

            BindingContext = new MainPageViewModel(this, mediaService);


        }

    }
}
