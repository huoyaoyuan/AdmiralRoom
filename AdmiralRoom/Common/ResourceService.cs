using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using Meowtrix.WPF.Extend;

namespace Huoyaoyuan.AdmiralRoom
{
    public sealed class ResourceService
    {
        private ResourceService() { }
        public static IReadOnlyCollection<CultureInfo> SupportedCultures { get; } =
            new[] { "zh-Hans", "ja", "en" }
            .Select(CultureInfo.GetCultureInfo)
            .Where(x => x != null)
            .ToList();
        private static CultureInfo _currentCulture;
        public static CultureInfo CurrentCulture
        {
            get { return _currentCulture; }
            set
            {
                var oldCulture = _currentCulture;
                _currentCulture = value;
                var temp1 = CurrentCultureChanged;
                CurrentCultureChanged?.Invoke(null, EventArgs.Empty);
                var temp = CultureChanged;
                temp?.Invoke(null, new PropertyChangedEventArgs<CultureInfo>(oldCulture, value));
            }
        }
        /// <summary>
        /// Only used for binding.
        /// </summary>
        public static event EventHandler CurrentCultureChanged;
        public static string GetString(string key) => App.Current.TryFindResource("LocalizedString_" + key)?.ToString() ?? key;
        public static void ChangeCulture(string CultureName)
        {
            CultureInfo culture = SupportedCultures.SingleOrDefault(x => x.Name == CultureName);
            if (culture != null)
            {
                //Properties.Resources.Culture = culture;
                CurrentCulture = culture;
            }
        }
        public static event EventHandler<PropertyChangedEventArgs<CultureInfo>> CultureChanged;
        public static void SetStringTableBinding(DependencyObject @object, DependencyProperty property, string key)
        {
            SetStringFromTable(@object, property, key);
            WeakEventManager<ResourceService, PropertyChangedEventArgs<CultureInfo>>.AddHandler(null, nameof(CultureChanged),
                (_, __) => SetStringFromTable(@object, property, key));
        }
        public static void SetStringFromTable(DependencyObject @object, DependencyProperty property, string key)
            => @object.SetCurrentValue(property, GetString(key));
    }
}
