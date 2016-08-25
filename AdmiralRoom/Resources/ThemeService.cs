using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Meowtrix.WPF.Extend;

namespace Huoyaoyuan.AdmiralRoom
{
    internal static class ThemeService
    {
        public static IReadOnlyCollection<string> SystemThemes { get; } = new[]
        {
            "Classic",
            "Luna.NormalColor",
            "Luna.Metallic",
            "Luna.HomeStead",
            "Royale",
            "Aero",
            "Aero2"
        };
        public static void SetSystemTheme(string name)
        {
            var a = name.Split('.');
            string themename = a[0];
            string colorname = "NormalColor";
            if (a.Length >= 2) colorname = a[1];
            if (themename.ToLower() == "classic") colorname = string.Empty;
            SystemThemeHelper.SetTheme(themename, colorname);
        }
        public static System.Collections.IEnumerable FontSource { get; } = Fonts.SystemFontFamilies.OrderBy(x => x.FamilyNames.Values.First()).Select(x => new { Font = x, Name = x.FamilyNames.Values.Last() }).ToList();
        /// <summary>
        /// A key that should be contained in every theme-specific <see cref="ResourceDictionary"/>.
        /// </summary>
        public static object ThemeDictionaryKey { get; } = new object();
        public static IReadOnlyCollection<string> Themes { get; } = new[]
        {
            "Default",
            "VS2013Dark"
        };
        private static string _currenttheme;
        public static string CurrentTheme
        {
            get { return _currenttheme; }
            set
            {
                if (!Themes.Contains(value)) return;
                if (_currenttheme != value)
                {
                    _currenttheme = value;
                    ChangeThemeCore(value);
                }
            }
        }
        private static void ChangeThemeCore(string theme)
        {
            foreach (var d in Application.Current.Resources.MergedDictionaries.Where(x => x.Contains(ThemeDictionaryKey)).ToList())
                Application.Current.Resources.MergedDictionaries.Remove(d);
            var dict = Application.LoadComponent(new Uri($"AdmiralRoom;component/Resources/{theme}Theme.xaml", UriKind.Relative)) as ResourceDictionary;
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }
}
