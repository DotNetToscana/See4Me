using Microsoft.Practices.ServiceLocation;
using See4Me.Localization.Resources;
using See4Me.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace See4Me.Extensions
{
    public static class ExceptionExtensions
    {
        private static ISettingsService settings;

        static ExceptionExtensions()
        {
            settings = ServiceLocator.Current.GetInstance<ISettingsService>();
        }

        public static string GetExceptionMessage(this Exception exception)
        {
            var error = AppResources.RecognitionError;

            if (settings.ShowExceptionOnError)
                error = $"{error} ({exception.Message})";

            return error;
        }
    }
}
