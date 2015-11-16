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
            if (para == "local")
            {
                return time.ToLocalTime();
            }
            else if (para == "remain")
            {
                return time.Remain();
            }
            else throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidCastException();
        }
    }
}
