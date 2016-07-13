using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;

namespace See4Me.Services
{
    public class NavigationService : INavigationService
    {
        private Template10.Services.NavigationService.INavigationService navigationService;
        private Template10.Services.NavigationService.INavigationService Navigator
        {
            get
            {
                if (navigationService == null)
                    navigationService = WindowWrapper.Current().NavigationServices.FirstOrDefault();

                return navigationService;
            }
        }

        public void GoBack()
        {
            if (Navigator.CanGoBack)
                Navigator.GoBack();
        }

        public void NavigateTo(string pageKey) => NavigateTo(pageKey, null);

        public void NavigateTo(string pageKey, object parameter)
        {
            Pages key;
            if (Enum.TryParse<Pages>(pageKey, out key))
                Navigator.Navigate(key, null);
        }
    }
}
