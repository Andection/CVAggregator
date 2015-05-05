using System;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CVAggregator.Host.Converters
{
    public class UriImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var photoUri = value as string;

            try
            {
                if (string.IsNullOrEmpty(photoUri))
                {
                    return GetDefaultImageSource();
                }

                try
                {
                    return GetImageSource(new Uri(photoUri));
                }
                catch (Exception ex)
                {
                    //todo: log it
                    return GetDefaultImageSource();
                }
            }
            catch (Exception ex)
            {
                //todo: log it
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static ImageSource GetImageSource(Uri uri)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource = uri;
            img.EndInit();
            return img;
        }

        private ImageSource GetDefaultImageSource()
        {
            return GetImageSource(GetDefaultUri());
        }

        private Uri GetDefaultUri()
        {
            return new Uri(@"pack://application:,,,/"
                           + Assembly.GetExecutingAssembly().GetName().Name
                           + ";component/"
                           + "Images/no_photo.png", UriKind.Absolute);
        }
    }
}