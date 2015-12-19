using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Huoyaoyuan.AdmiralRoom.Officer;

namespace Huoyaoyuan.AdmiralRoom.Views.Converter
{
    public class LVColorConverter : IValueConverter
    {
        #region Constant HP Brushes
        private static SolidColorBrush HP100Brush = new SolidColorBrush(Colors.SpringGreen).TryFreeze();
        private static SolidColorBrush HP75Brush = new SolidColorBrush(Colors.Aquamarine).TryFreeze();
        private static SolidColorBrush HP50Brush = new SolidColorBrush(Colors.GreenYellow).TryFreeze();
        private static SolidColorBrush HP25Brush = new SolidColorBrush(Colors.Orange).TryFreeze();
        private static SolidColorBrush HP0Brush = new SolidColorBrush(Colors.Red).TryFreeze();
        #endregion

        #region Constant Quest Brushes
        private static SolidColorBrush Quest0Brush = new SolidColorBrush(Colors.Orange).TryFreeze();
        private static SolidColorBrush Quest50Brush = new SolidColorBrush(Colors.LawnGreen).TryFreeze();
        private static SolidColorBrush Quest80Brush = new SolidColorBrush(Colors.LimeGreen).TryFreeze();
        private static SolidColorBrush Quest100Brush = new SolidColorBrush(Colors.MediumTurquoise).TryFreeze();
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
            if (param == "hp")
            {
                if (v.IsMax) return HP100Brush;
                else if (v.Current * 4 > v.Max * 3) return HP75Brush;
                else if (v.Current * 2 > v.Max) return HP50Brush;
                else if (v.Current * 4 > v.Max) return HP25Brush;
                else return HP0Brush;
            }
            else if (param == "aircraft")
            {
                if (v.Max == 0) return new SolidColorBrush(Color.FromRgb(0, 128, 0)).TryFreeze();
                return new SolidColorBrush(Color.FromRgb(
                    (byte)(255 * v.Shortage / v.Max),
                    (byte)(128 * v.Current / v.Max),
                    1)).TryFreeze();
            }
            else if (param == "quest")
            {
                if (v.IsMax) return Quest100Brush;
                else if (v.Current * 5 >= v.Max * 4) return Quest80Brush;
                else if (v.Current * 2 >= v.Max) return Quest50Brush;
                else return Quest0Brush;
            }
            else throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidCastException();
        }
    }
}
