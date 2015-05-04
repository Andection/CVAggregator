using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CVAggregator.Host.Converters
{
    public class NotEmptyStringVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;

            return !string.IsNullOrWhiteSpace(stringValue) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}