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
            .Select(x =>
            {
                try { return CultureInfo.GetCultureInfo(x); }
                catch { return null; }
            })
            .Where(x => x != null)
            .ToList();

        public Properties.Resources Resources { get; } = new Properties.Resources();
        public void ChangeCulture(string CultureName)
        {
            CultureInfo culture = SupportedCultures.SingleOrDefault(x => x.Name == CultureName);
            if (culture != null) Properties.Resources.Culture = culture;
            OnPropertyChanged("Resources");
            CultureChanged?.Invoke(this, new EventArgs());
        }
        public event EventHandler CultureChanged;
    }
}
