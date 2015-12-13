using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Huoyaoyuan.AdmiralRoom.Views.Converter
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v = (bool)value;
            bool para = true;
            var paramarray = parameter?.ToString().Split('|');
            if (parameter != null) para = bool.Parse(paramarray[0]);
            if (v == para) return Visibility.Visible;
            else if ((paramarray?.Length ?? 0) <= 1) return Visibility.Collapsed;
            else if (paramarray[1].ToLower() == "hidden") return Visibility.Hidden;
            else throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }
}
