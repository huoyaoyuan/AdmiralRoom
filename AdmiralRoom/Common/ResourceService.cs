using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Huoyaoyuan.AdmiralRoom
{
    internal class ResourceService : NotificationObject
    {
        private ResourceService() { }
        public static ResourceService Current { get; } = new ResourceService();
        public static IReadOnlyCollection<CultureInfo> SupportedCultures { get; } =
            new[] { "zh-Hans", "ja", "en" }
#pragma warning disable CC0020 // You should remove the lambda expression when it only invokes a method with the same signature
            .Select(x =>
            {
                try { return CultureInfo.GetCultureInfo(x); }
                catch { return null; }
            })
#pragma warning restore CC0020 // You should remove the lambda expression when it only invokes a method with the same signature
            .Where(x => x != null)
            .ToList();

        public Properties.Resources Resources { get; } = new Properties.Resources();
        public void ChangeCulture(string CultureName)
        {
            CultureInfo culture = SupportedCultures.SingleOrDefault(x => x.Name == CultureName);
            if (culture != null) Properties.Resources.Culture = culture;
            OnPropertyChanged(nameof(Resources));
            CultureChanged?.Invoke(culture);
        }
        public event Action<CultureInfo> CultureChanged;
    }
}
