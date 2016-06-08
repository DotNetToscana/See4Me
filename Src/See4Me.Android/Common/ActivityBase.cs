using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using See4Me.ViewModels;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Messaging;
using Android.OS;
using GalaSoft.MvvmLight.Views;

namespace See4Me.Android.Common
{
    public abstract class ActivityBase<T> : GalaSoft.MvvmLight.Views.ActivityBase where T : ViewModelBase
    {
        protected T ViewModel => ServiceLocator.Current.GetInstance<T>();

        protected virtual int? LayoutResourseID { get; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (LayoutResourseID.HasValue)
            {
                SetContentView(LayoutResourseID.Value);
            }
            else
            {
                var view = OnSetContentView(savedInstanceState);
                if (view != null)
                    SetContentView(view);
            }

            OnInitialize(savedInstanceState);
        }

        protected virtual View OnSetContentView(Bundle savedInstanceState) => null;

        protected override void OnDestroy()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister(this);
            base.OnDestroy();
        }

        protected virtual void OnInitialize(Bundle bundle) { }
    }
}