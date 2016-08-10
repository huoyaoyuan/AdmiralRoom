using System;
using System.Globalization;
using System.Windows.Data;

namespace Huoyaoyuan.AdmiralRoom.Views.Converter
{
    public sealed class StringTableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ResourceService.GetString(parameter.ToString());

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
