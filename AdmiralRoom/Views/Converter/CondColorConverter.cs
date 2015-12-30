using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

#pragma warning disable CC0013

namespace Huoyaoyuan.AdmiralRoom.Views.Converter
{
    public class CondColorConverter : IValueConverter
    {
        #region ConstantBrushes
        private static readonly SolidColorBrush Brush20Foreground = new SolidColorBrush(Colors.Red).TryFreeze();
        private static readonly SolidColorBrush Brush30Foreground = new SolidColorBrush(Colors.Orange).TryFreeze();
        private static readonly SolidColorBrush Brush40Foreground = new SolidColorBrush(Colors.DarkGray).TryFreeze();
        private static readonly SolidColorBrush Brush50Foreground = new SolidColorBrush(Colors.DarkCyan).TryFreeze();
        private static readonly SolidColorBrush Brush85Foreground = new SolidColorBrush(Colors.MediumAquamarine).TryFreeze();
        private static readonly SolidColorBrush BrushMaxForeground = new SolidColorBrush(Colors.MediumSpringGreen).TryFreeze();
        private static readonly SolidColorBrush Brush20Background = new SolidColorBrush(Color.FromArgb(96, 255, 0, 0)).TryFreeze();
        private static readonly SolidColorBrush Brush30Background = new SolidColorBrush(Color.FromArgb(96, 255, 96, 0)).TryFreeze();
        private static readonly SolidColorBrush Brush40Background = new SolidColorBrush(Color.FromArgb(48, 255, 128, 0)).TryFreeze();
        private static readonly SolidColorBrush Brush50Background = new SolidColorBrush(Colors.Transparent).TryFreeze();
        private static readonly SolidColorBrush BrushMaxBackground = new SolidColorBrush(Color.FromArgb(192, 255, 255, 0)).TryFreeze();
        #endregion

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isforeground = bool.Parse(parameter.ToString());
            int cond = (int)value;
            if (cond < 20) return isforeground ? Brush20Foreground : Brush20Background;
            else if (cond < 30) return isforeground ? Brush30Foreground : Brush30Background;
            else if (cond < 40) return isforeground ? Brush40Foreground : Brush40Background;
            else if (cond < 50) return isforeground ? Brush50Foreground : Brush50Background;
            else if (cond <= 85) return isforeground ? Brush85Foreground : new SolidColorBrush(Color.FromArgb((byte)(96 + (cond - 50) / 3 * 4), 255, 255, 0)).TryFreeze();
            else return isforeground ? BrushMaxForeground : BrushMaxBackground;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidCastException();
        }
    }
}
