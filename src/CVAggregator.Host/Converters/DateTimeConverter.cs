using System;
using System.Globalization;
using System.Windows.Data;

namespace CVAggregator.Host.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            var date = (DateTime) value;

            return date.Date == DateTime.Today ? string.Format("Today, {0:t}", date) : date.ToString("yyyy MMMM dd");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}