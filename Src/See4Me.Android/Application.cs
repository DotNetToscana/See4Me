using System;
using System.IO;
using Android.App;
using Android.Runtime;
using See4Me.ViewModels;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;

namespace See4Me.Android
{
    [Application(Label = "See4Me", Icon = "@drawable/Icon")]
    public class Application : global::Android.App.Application
    {
        /// <summary>
        /// Must implement this constructor for subclassing the application class.
        /// Will act as a global application class throughout the app.
        /// </summary>
        /// <param name="javaReference">pointer to java</param>
        /// <param name="transfer">transfer enumeration</param>
        public Application(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        { }

        /// <summary>
        /// Override on create to instantiate the service container to be persistant.
        /// </summary>
        public override void OnCreate()
        {
            base.OnCreate();

            // Registers services for core library
            this.Initialize();
        }

        private void Initialize()
        {
            // Page navigation
            var navigationService = new NavigationService();
            navigationService.Configure(Constants.MainPage, typeof(MainActivity));

            this.Locator = new ViewModelLocator();
            Locator.Initialize(navigationService);

            DispatcherHelper.Initialize();
        }

        public ViewModelLocator Locator { get; private set; }
    }
}