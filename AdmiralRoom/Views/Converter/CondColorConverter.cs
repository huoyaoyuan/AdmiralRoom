using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Huoyaoyuan.AdmiralRoom.Views.Converter
{
    public class CondColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isforeground = bool.Parse(parameter.ToString());
            int cond = (int)value;
            Color c;
            if (cond < 20) c = isforeground ? Colors.Red : Color.FromArgb(96, 255, 0, 0);
            else if (cond < 30) c = isforeground ? Colors.Orange : Color.FromArgb(96, 255, 96, 0);
            else if (cond < 40) c = isforeground ? Colors.DarkGray : Color.FromArgb(48, 255, 128, 0);
            else if (cond < 50) c = isforeground ? Colors.DarkCyan : Colors.Transparent;
            else if (cond <= 85) c = isforeground ? Colors.MediumAquamarine : Color.FromArgb((byte)(96 + (cond - 50) / 3 * 4), 255, 255, 0);
            else c = isforeground ? Colors.MediumSpringGreen : Color.FromArgb(192, 255, 255, 0);
            return new SolidColorBrush(c);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidCastException();
        }
    }
}
