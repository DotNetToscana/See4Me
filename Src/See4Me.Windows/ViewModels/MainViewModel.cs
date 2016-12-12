﻿using See4Me.Common;
using See4Me.Services;
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
            await this.InitializeStreamingAsync();
            await this.CheckShowConsentAsync();

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
