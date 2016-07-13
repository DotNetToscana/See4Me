using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Services
{
    public enum Pages { MainPage, SettingsPage, AboutPage, RecognizeTextPage }

    public interface INavigationService
    {
        void GoBack();

        void NavigateTo(string pageKey);

        void NavigateTo(string pageKey, object parameter);
    }
}
