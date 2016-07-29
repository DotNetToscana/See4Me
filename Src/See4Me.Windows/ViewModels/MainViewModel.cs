using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.ProjectOxford.Vision;
using See4Me.Common;
using See4Me.Services;
using See4Me.Services.Translator;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Template10.Services.NavigationService;
using Microsoft.Practices.ServiceLocation;
using See4Me.Localization.Resources;

namespace See4Me.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            // Calls methods to initialize the app.
            this.InitializeServices();
            await this.InitializeAsync();

            // If not given, asks the user for the consent to use the app.
            if (!Settings.IsConsentGiven)
            {
                await DialogService.ShowAsync(AppResources.ConsentRequiredMessage, AppResources.ConsentRequiredTitle);
                Settings.IsConsentGiven = true;
            }

            DescribeImageCommand.RaiseCanExecuteChanged();
            SwapCameraCommand.RaiseCanExecuteChanged();
            GotoRecognizeTextCommand.RaiseCanExecuteChanged();

            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            // When navigating away from this page (even for suspending), cleanup the associated resources.
            await this.CleanupAsync();

            await base.OnNavigatedFromAsync(state, suspending);
        }
    }
}
