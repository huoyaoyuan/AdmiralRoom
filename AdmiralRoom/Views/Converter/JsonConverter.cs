using System;
using System.Globalization;
using System.Windows.Data;
using Newtonsoft.Json.Linq;

namespace Huoyaoyuan.AdmiralRoom.Views.Converter
{
    class JsonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (parameter.ToString().ToLowerInvariant())
            {
                case "children":
                    if (value is JProperty)
                    {
                        var p = value as JProperty;
                        return p.Value.Children();
                    }
                    else return ((JToken)value).Children();
                case "value":
                    JToken v = value as JToken;
                    if (v is JProperty)
                    {
                        var p = v as JProperty;
                        if (p.Value is JContainer) return p.Name;
                        else return p.Name + ":" + p.Value;
                    }
                    else
                    {
                        if (v.Parent is JArray)
                            if (v is JObject || v is JArray)
                                return $"[{(v.Parent as JArray).IndexOf(v)}]";
                            else return $"[{(v.Parent as JArray).IndexOf(v)}]:{v}";
                        else if (v is JObject) return "{}";
                        else if (v is JArray) return "[]";
                        else return v.ToString();
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
