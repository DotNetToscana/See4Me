using See4Me.Common;
using System;
using Windows.UI.Xaml.Data;

namespace See4Me.Converters
{
    public class SwipeDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            SwipeDirection direction;
            if (parameter != null && Enum.TryParse<SwipeDirection>(parameter.ToString(), out direction))
                return direction;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
