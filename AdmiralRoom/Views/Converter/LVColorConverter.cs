using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Huoyaoyuan.AdmiralRoom.Officer;

namespace Huoyaoyuan.AdmiralRoom.Views.Converter
{
    public class LVColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string param = parameter.ToString().ToLower();
            LimitedValue v;
            try
            {
                v = (LimitedValue)value;
            }
            catch (InvalidCastException) { return null; }
            if (param == "hp")
            {
                if (v.IsMax) return new SolidColorBrush(Colors.SpringGreen);
                else if (v.Current * 4 > v.Max * 3) return new SolidColorBrush(Colors.Aquamarine);
                else if (v.Current * 2 > v.Max) return new SolidColorBrush(Colors.GreenYellow);
                else if (v.Current * 4 > v.Max) return new SolidColorBrush(Colors.Orange);
                else return new SolidColorBrush(Colors.Red);
            }
            else if (param == "aircraft")
            {
                if (v.Max == 0) return new SolidColorBrush(Color.FromRgb(0, 128, 0));
                return new SolidColorBrush(Color.FromRgb(
                    (byte)(255 * v.Shortage / v.Max),
                    (byte)(128 * v.Current / v.Max),
                    1));
            }
            else if (param == "quest")
            {
                if (v.IsMax) return new SolidColorBrush(Colors.MediumTurquoise);
                else if (v.Current * 5 >= v.Max * 4) return new SolidColorBrush(Colors.LimeGreen);
                else if (v.Current * 2 >= v.Max) return new SolidColorBrush(Colors.LawnGreen);
                else return new SolidColorBrush(Colors.Orange);
            }
            else throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidCastException();
        }
    }
}
