using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using See4Me.Services;

namespace See4Me.ViewModels
{
    public class SplashViewModel : ViewModelBase
    {
        private readonly Services.INavigationService navigationService;

        public SplashViewModel(Services.INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        public void NavigateToMainPage()
        {
            navigationService.NavigateTo(Pages.MainPage.ToString());
        }
    }
}
