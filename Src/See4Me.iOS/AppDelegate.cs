using Foundation;
using GalaSoft.MvvmLight.Threading;
using See4Me.Services;
using See4Me.ViewModels;
using UIKit;

namespace See4Me.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        public ViewModelLocator Locator { get; private set; }
		public override UIWindow Window { get; set; }
		public static UIStoryboard Storyboard = UIStoryboard.FromName("Main", null);
		public static UINavigationController initialController;

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            this.Initialize(application);

			//Uncomment the 3 rows below to enforce English
			//var ci = new System.Globalization.CultureInfo("en");
			//System.Threading.Thread.CurrentThread.CurrentCulture = ci;
			//System.Threading.Thread.CurrentThread.CurrentUICulture = ci;

            return true;
        }

        private void Initialize(UIApplication application)
        {
			this.Window = new UIWindow(UIScreen.MainScreen.Bounds);

            var navigationService = new See4Me.Services.NavigationService();

            this.Locator = new ViewModelLocator();
            this.Locator.Initialize(navigationService);

			initialController = Storyboard.InstantiateInitialViewController() as UINavigationController;
			navigationService.Initialize(initialController);
			navigationService.Configure(Pages.MainPage.ToString(), Pages.MainPage.ToString());
			navigationService.Configure(Pages.SettingsPage.ToString(), Pages.SettingsPage.ToString());
			navigationService.Configure(Pages.AboutPage.ToString(), Pages.AboutPage.ToString());
			navigationService.Configure(Pages.PrivacyPolicyPage.ToString(), Pages.PrivacyPolicyPage.ToString());

			Window.RootViewController = initialController;
			Window.MakeKeyAndVisible();
            // MVVM Light's DispatcherHelper for cross-thread handling.
            DispatcherHelper.Initialize(application);
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message)
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive.
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }
    }
}