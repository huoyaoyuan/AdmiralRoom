using System;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;

namespace Huoyaoyuan.AdmiralRoom.Officer.Converter
{
    class TransparencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = value as SolidColorBrush;
            var para = double.Parse(parameter.ToString());
            var c = v.Color;
            c.A = (byte)(c.A * para);
            return new SolidColorBrush(c);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = value as SolidColorBrush;
            var para = double.Parse(parameter.ToString());
            var c = v.Color;
            c.A = (byte)(c.A / para);
            return new SolidColorBrush(c);
        }
    }
}
