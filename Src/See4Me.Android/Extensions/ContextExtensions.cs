using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Command;

namespace See4Me.Android.Extensions
{
    public static class ContextExtensions
    {
        public static Binding<TSource, TTarget> RegisterHandler<TSource, TTarget>(this Binding<TSource, TTarget> binding, View view)
        {
            if (view is TextView)
            {
                var textView = (view as TextView);
                textView.Text = null;
                textView.TextChanged += (s, e) => { };
            }
            else if (view is CheckBox)
            {
                (view as CheckBox).CheckedChange += (s, e) => { };
            }

            return binding;
        }

        public static View SetCommand(this View view, RelayCommand command)
        {
            view.Click += (s, e) => { };
            view.SetCommand(Events.Click, command);

            //view.Enabled = command.CanExecute(null);
            //command.CanExecuteChanged += (s, args) => view.Enabled = command.CanExecute(null);

            return view;
        }
    }

    public static class Events
    {
        public const string TextChanged = "TextChanged";
        public const string CheckedChanged = "CheckedChanged";
        public const string Click = "Click";
    }
}