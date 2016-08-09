using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Meowtrix.ComponentModel;

namespace Huoyaoyuan.AdmiralRoom
{
    internal class ResourceService : NotificationObject
    {
        private ResourceService() { }
        public static ResourceService Current { get; } = new ResourceService();
        public static IReadOnlyCollection<CultureInfo> SupportedCultures { get; } =
            new[] { "zh-Hans", "ja", "en" }
            .Select(CultureInfo.GetCultureInfo)
            .Where(x => x != null)
            .ToList();
        private CultureInfo _currentCulture;
        public CultureInfo CurrentCulture
        {
            get { return _currentCulture; }
            set
            {
                _currentCulture = value;
                OnAllPropertyChanged();
                CultureChanged?.Invoke(value);
            }
        }
        public Properties.Resources Resources { get; } = new Properties.Resources();
        public void ChangeCulture(string CultureName)
        {
            CultureInfo culture = SupportedCultures.SingleOrDefault(x => x.Name == CultureName);
            if (culture != null)
            {
                Properties.Resources.Culture = culture;
                CurrentCulture = culture;
            }
        }
        public event Action<CultureInfo> CultureChanged;
    }
}
