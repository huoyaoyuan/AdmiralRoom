using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Huoyaoyuan.AdmiralRoom.Officer;

#pragma warning disable RECS0147
#pragma warning disable CC0013

namespace Huoyaoyuan.AdmiralRoom.Views.Converter
{
    public class LVColorConverter : IValueConverter
    {
        #region Constant HP Brushes
        private static readonly SolidColorBrush HP100Brush = new SolidColorBrush(Colors.SpringGreen).TryFreeze();
        private static readonly SolidColorBrush HP75Brush = new SolidColorBrush(Colors.Aquamarine).TryFreeze();
        private static readonly SolidColorBrush HP50Brush = new SolidColorBrush(Colors.GreenYellow).TryFreeze();
        private static readonly SolidColorBrush HP25Brush = new SolidColorBrush(Colors.Orange).TryFreeze();
        private static readonly SolidColorBrush HP0Brush = new SolidColorBrush(Colors.Red).TryFreeze();
        #endregion

        #region Constant Quest Brushes
        private static readonly SolidColorBrush Quest0Brush = new SolidColorBrush(Colors.Orange).TryFreeze();
        private static readonly SolidColorBrush Quest50Brush = new SolidColorBrush(Colors.LawnGreen).TryFreeze();
        private static readonly SolidColorBrush Quest80Brush = new SolidColorBrush(Colors.LimeGreen).TryFreeze();
        private static readonly SolidColorBrush Quest100Brush = new SolidColorBrush(Colors.MediumTurquoise).TryFreeze();
        #endregion

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
                    if (v.IsMax) return HP100Brush;
                    else if (v.Current * 4 > v.Max * 3) return HP75Brush;
                    else if (v.Current * 2 > v.Max) return HP50Brush;
                    else if (v.Current * 4 > v.Max) return HP25Brush;
                    else return HP0Brush;
                case "aircraft":
                    if (v.Max == 0) return new SolidColorBrush(Color.FromRgb(0, 128, 0)).TryFreeze();
                    return new SolidColorBrush(Color.FromRgb(
                        (byte)(255 * v.Shortage / v.Max),
                        (byte)(128 * v.Current / v.Max),
                        1)).TryFreeze();
                case "quest":
                    if (v.IsMax) return Quest100Brush;
                    else if (v.Current * 5 >= v.Max * 4) return Quest80Brush;
                    else if (v.Current * 2 >= v.Max) return Quest50Brush;
                    else return Quest0Brush;
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
