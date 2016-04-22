using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using See4Me.ViewModels;
using GalaSoft.MvvmLight.Views;

namespace See4Me.iOS.Common
{
    public abstract class ViewControllerBase<T> : ControllerBase where T : ViewModelBase
    {
        protected T ViewModel => ServiceLocator.Current.GetInstance<T>();

        public ViewControllerBase(IntPtr handle) : base (handle)
        { }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister(this);
        }
    }
}
