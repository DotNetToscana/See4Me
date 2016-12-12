using See4Me.Common;
using See4Me.Localization.Resources;
using See4Me.Services;
using System;


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
