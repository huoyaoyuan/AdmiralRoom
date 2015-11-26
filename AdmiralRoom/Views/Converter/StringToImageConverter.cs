using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Huoyaoyuan.AdmiralRoom.Views.Converter
{
    class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string source = value.ToString();
            if (parameter != null)
                source = string.Format(parameter.ToString(), source);
            try
            {
                return new BitmapImage { UriSource = new Uri(source, UriKind.Relative) };
            }
            catch { return null; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidCastException();
        }
    }
}
