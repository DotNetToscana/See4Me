using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Vision;
using See4Me.Common;
using See4Me.Localization.Resources;
using See4Me.Services;
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
    public partial class AboutViewModel : ViewModelBase
    {
        private readonly ILauncherService launcherService;
        private readonly IAppService appService;

        public string BlogUrl => Constants.BlogUrl;

        public string TwitterUrl => Constants.TwitterUrl;

        public string LinkedInUrl => Constants.LinkedInUrl;

        public string CognitiveServicesUrl => Constants.CognitiveServicesUrl;

        public AutoRelayCommand GotoGitHubCommand { get; set; }

        public AutoRelayCommand GotoPrivacyPolicyCommand { get; set; }

        public AutoRelayCommand<string> GotoUrlCommand { get; set; }

        public string AppVersion => appService.Version;

        public AboutViewModel(ILauncherService launcherService, IAppService appService)
        {
            this.launcherService = launcherService;
            this.appService = appService;

            this.CreateCommands();
        }

        private void CreateCommands()
        {
            GotoGitHubCommand = new AutoRelayCommand(() => launcherService.LaunchUriAsync(Constants.GitHubProjectUrl));
            GotoUrlCommand = new AutoRelayCommand<string>((url) => launcherService.LaunchUriAsync(url));
            GotoPrivacyPolicyCommand = new AutoRelayCommand(() => Navigator.NavigateTo(Pages.PrivacyPolicyPage.ToString()));
        }
    }
}
