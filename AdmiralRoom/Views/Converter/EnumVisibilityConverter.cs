using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Huoyaoyuan.AdmiralRoom.Views.Converter
{
    public class EnumVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value.ToString().ToLower() == parameter.ToString().ToLower() ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            (Visibility)parameter == Visibility.Visible ? Enum.Parse(targetType, parameter.ToString()) : DependencyProperty.UnsetValue;
    }
}
