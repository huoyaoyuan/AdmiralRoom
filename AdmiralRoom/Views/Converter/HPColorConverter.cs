using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Huoyaoyuan.AdmiralRoom.Officer;

namespace Huoyaoyuan.AdmiralRoom.Views.Converter
{
    public class HPColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                LimitedValue HP = (LimitedValue)value;
                if (HP.IsMax) return new SolidColorBrush(Colors.SpringGreen);
                else if (HP.Current * 4 > HP.Max * 3) return new SolidColorBrush(Colors.Aquamarine);
                else if (HP.Current * 2 > HP.Max) return new SolidColorBrush(Colors.GreenYellow);
                else if (HP.Current * 4 > HP.Max) return new SolidColorBrush(Colors.Orange);
                else return new SolidColorBrush(Colors.Red);
            }
            catch { return new SolidColorBrush(Colors.Green); }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
