using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Huoyaoyuan.AdmiralRoom.Views.Converter
{
    class NotNullVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool para = true;
            if (parameter != null) para = bool.Parse(parameter.ToString());
            return value == null ^ para ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidCastException();
        }
    }
}
