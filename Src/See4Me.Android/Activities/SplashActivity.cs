using Android.App;
using Android.Content;
using Android.OS;
using Android.Content.PM;
using GalaSoft.MvvmLight.Views;
using See4Me.Android;
using System.Threading.Tasks;
using See4Me.ViewModels;
using See4Me.Android.Common;

namespace SpeedOrder.Droid.Activities
{
    [Activity(MainLauncher = true, Label = "@string/ApplicationName", Theme = "@style/See4Me.Splash", Icon = "@drawable/icon",
        ScreenOrientation = ScreenOrientation.Landscape, NoHistory = true)]
    public class SplashActivity : ActivityBase<SplashViewModel>
    {
        protected override async void OnResume()
        {
            base.OnResume();

            // Uses a delay to force the splash screen to appear.
            await Task.Delay(100);

            // Navigates to the actual Main Page.
            ViewModel.NavigateToMainPage();
        }
    }
}