using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Huoyaoyuan.AdmiralRoom.Officer;

#pragma warning disable CC0013

namespace Huoyaoyuan.AdmiralRoom.Views.Converter
{
    public sealed class LVColorConverter : IValueConverter
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

            switch (param)
            {
                case "hp":
                    if (v.IsMax) return Brushes.SpringGreen;
                    else if (v.Current * 4 > v.Max * 3) return Brushes.Aquamarine;
                    else if (v.Current * 2 > v.Max) return Brushes.GreenYellow;
                    else if (v.Current * 4 > v.Max) return Brushes.Orange;
                    else return Brushes.Red;
                case "aircraft":
                    if (v.Max == 0) return new SolidColorBrush(Color.FromRgb(0, 128, 0)).TryFreeze();
                    return new SolidColorBrush(Color.FromRgb(
                        (byte)(255 * v.Shortage / v.Max),
                        (byte)(128 * v.Current / v.Max),
                        1)).TryFreeze();
                case "quest":
                    if (v.IsMax) return Brushes.MediumTurquoise;
                    else if (v.Current * 5 >= v.Max * 4) return Brushes.LimeGreen;
                    else if (v.Current * 2 >= v.Max) return Brushes.LawnGreen;
                    else return Brushes.Orange;
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
