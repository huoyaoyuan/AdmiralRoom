using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using Meowtrix.ComponentModel;
using Meowtrix.WPF.Extend;

namespace Huoyaoyuan.AdmiralRoom
{
    public class ResourceService : NotificationObject
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
                var oldCulture = _currentCulture;
                _currentCulture = value;
                OnAllPropertyChanged();
                var temp = CultureChanged;
                temp?.Invoke(this, new PropertyChangedEventArgs<CultureInfo>(oldCulture, value));
            }
        }
        public static string GetString(string key) => App.Current.TryFindResource("LocalizedString_" + key)?.ToString() ?? key;
        public void ChangeCulture(string CultureName)
        {
            CultureInfo culture = SupportedCultures.SingleOrDefault(x => x.Name == CultureName);
            if (culture != null)
            {
                //Properties.Resources.Culture = culture;
                CurrentCulture = culture;
            }
        }
        public event EventHandler<PropertyChangedEventArgs<CultureInfo>> CultureChanged;
        public void SetStringTableBinding(DependencyObject @object, DependencyProperty property, string key)
        {
            SetStringFromTable(@object, property, key);
            WeakEventManager<ResourceService, PropertyChangedEventArgs<CultureInfo>>.AddHandler(this, nameof(CultureChanged),
                (_, __) => SetStringFromTable(@object, property, key));
        }
        public static void SetStringFromTable(DependencyObject @object, DependencyProperty property, string key)
            => @object.SetCurrentValue(property, GetString(key));
    }
}
