using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Vision;
using See4Me.Common;
using See4Me.Localization.Resources;
using See4Me.Services;
using See4Me.Services.Translator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using See4Me.Extensions;
using System.IO;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.ProjectOxford.Vision.Contract;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Ioc;

namespace See4Me.ViewModels
{
    public partial class PrivacyViewModel : ViewModelBase
    {
        private readonly ILauncherService launcherService;

        public AutoRelayCommand GotoCognitiveServicesUrlCommand { get; set; }

        public AutoRelayCommand GotoMicrosoftPrivacyPoliciesUrlCommand { get; set; }

        public PrivacyViewModel(ILauncherService launcherService, IAppService appService)
        {
            this.launcherService = launcherService;

            this.CreateCommands();
        }

        private void CreateCommands()
        {
            GotoCognitiveServicesUrlCommand = new AutoRelayCommand(() => launcherService.LaunchUriAsync(Constants.CognitiveServicesUrl));
            GotoMicrosoftPrivacyPoliciesUrlCommand = new AutoRelayCommand(() => launcherService.LaunchUriAsync(Constants.MicrosoftPrivacyPoliciesUrl));
        }
    }
}
