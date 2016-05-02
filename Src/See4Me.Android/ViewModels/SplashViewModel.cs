using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ServiceLocation;

namespace See4Me.ViewModels
{
    public class SplashViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;

        public SplashViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        public void NavigateToMainPage()
        {
            navigationService.NavigateTo(Constants.MainPage);
        }
    }
}
