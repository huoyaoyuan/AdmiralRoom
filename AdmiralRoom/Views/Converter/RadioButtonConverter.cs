using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Huoyaoyuan.AdmiralRoom.Views.Converter
{
    public class RadioButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value.ToString().ToLower() == parameter.ToString().ToLower();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ischecked = (bool)value;
            if (!ischecked) return DependencyProperty.UnsetValue;
            if (targetType == typeof(string)) return parameter.ToString();
            if (targetType.IsEnum) return Enum.Parse(targetType, parameter.ToString());
            throw new ArgumentException(nameof(targetType));
        }
    }
}
