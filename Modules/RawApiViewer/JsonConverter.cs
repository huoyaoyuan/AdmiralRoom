using System;
using System.Globalization;
using System.Windows.Data;
using Newtonsoft.Json.Linq;

namespace RawApiViewer
{
    class JsonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (parameter.ToString().ToLowerInvariant())
            {
                case "children":
                    return ((JToken)value).Children();
                case "value":
                    if (value is JProperty)
                    {
                        var p = value as JProperty;
                        if (p.Value is JContainer) return p.Name;
                        else return p.Name + ":" + p.Value;
                    }
                    else
                    {
                        if (value is JObject) return "{}";
                        else if (value is JArray) return "[]";
                        else return value.ToString();
                    }
                default:
                    throw new ArgumentException(nameof(parameter));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidCastException();
        }
    }
}
