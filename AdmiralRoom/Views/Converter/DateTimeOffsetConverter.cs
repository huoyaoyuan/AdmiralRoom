using System;
using System.Globalization;
using System.Windows.Data;

namespace Huoyaoyuan.AdmiralRoom.Views.Converter
{
    class DateTimeOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTimeOffset time = (DateTimeOffset)value;
            string para = parameter.ToString().ToLower();
            switch (para)
            {
                case "local":
                    return time.ToLocalTime();
                case "remain":
                    return time.Remain().ToDisplayString();
                case "during":
                    return time.During().ToDisplayString();
                default:
                    throw new ArgumentException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidCastException();
        }
    }
}
