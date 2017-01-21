using System;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using See4Me.Common;
using See4Me.Localization.Resources;
using See4Me.Services;

namespace See4Me.ViewModels
{
    public partial class AboutViewModel : ViewModelBase
    {
        private readonly ILauncherService launcherService;
        private readonly IAppService appService;

        public string BlogUrl => appService.BlogUrl;

        public string TwitterUrl => appService.TwitterUrl;

        public string LinkedInUrl => appService.LinkedInUrl;

        public string CognitiveServicesUrl => Constants.CognitiveServicesUrl;

        public AutoRelayCommand GotoGitHubCommand { get; set; }

        public AutoRelayCommand GotoPrivacyPolicyCommand { get; set; }

        public AutoRelayCommand<string> GotoUrlCommand { get; set; }

        public string AppVersion => appService.Version;

        public string ProjectAuthor => appService.Author;

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
            GotoPrivacyPolicyCommand = new AutoRelayCommand(() => AppNavigationService.NavigateTo(Pages.PrivacyPolicyPage.ToString()));
        }
    }
}
