using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Fluent;

namespace Huoyaoyuan.AdmiralRoom
{
    internal static class ThemeService
    {
        private static readonly Dictionary<string, string[]> themes = new Dictionary<string, string[]>
        {
            ["Office 2010 Silver"] = new[]
            {
                "Fluent;Component/Themes/Generic.xaml",
                "Fluent;Component/Themes/Office2010/Silver.xaml"
            },
            ["Office 2010 Black"] = new[]
            {
                "Fluent;Component/Themes/Generic.xaml",
                "Fluent;Component/Themes/Office2010/Black.xaml"
            },
            ["Office 2010 Blue"] = new[]
            {
                "Fluent;Component/Themes/Generic.xaml",
                "Fluent;Component/Themes/Office2010/Blue.xaml"
            },
            ["Office 2013"] = new[]
            {
                "Fluent;Component/Themes/Office2013/Generic.xaml"
            },
            ["Windows 8"] = new[]
            {
                "Fluent;component/Themes/Windows8/Generic.xaml",
                "Fluent;component/Themes/Windows8/Silver.xaml"
            }
        };
        public static IReadOnlyCollection<string> SupportedThemes => themes.Keys;
        public static void ChangeRibbonTheme(string theme, RibbonWindow window)
        {
            try
            {
                var themeres = themes[theme];
#pragma warning disable CC0108
                window.SetTheme("Ribbon", themeres);
#pragma warning restore CC0108
                window.Style = null;
                window.Style = window.FindResource("RibbonWindowStyle") as Style;
                window.Style = null;
                --window.Width;
                ++window.Width;
                Application.Current.MainWindow.SetThemePrior("Aero");
            }
            catch { ChangeRibbonTheme(SupportedThemes.First(), window); }
        }
        public static void SetDontUseDwm(bool dontuse)
        {
            var wnd = Application.Current.MainWindow as Fluent.RibbonWindow;
            if (wnd == null) return;
            if (dontuse)
            {
                wnd.DontUseDwm = true;
                wnd.WindowStyle = WindowStyle.None;
                wnd.AllowsTransparency = true;
            }
            else
            {
                wnd.AllowsTransparency = false;
                wnd.WindowStyle = WindowStyle.SingleBorderWindow;
                wnd.DontUseDwm = false;
            }
        }
        public static void EnableAeroControls(bool Enable)
        {
            if (Enable)
            {
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(App.LoadComponent(new Uri("PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35;component/themes/aero.normalcolor.xaml", UriKind.Relative)) as ResourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(App.LoadComponent(new Uri("AdmiralRoom;component/themes/aero.normalcolor.xaml", UriKind.Relative)) as ResourceDictionary);
                Application.Current.MainWindow.SetTheme("Aero", new[]
                {
                    "PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35;component/themes/aero.normalcolor.xaml",
                    "AdmiralRoom;component/themes/aero.normalcolor.xaml"
                });
            }
            else
            {
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(App.LoadComponent(new Uri("PresentationFramework.AeroLite, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35;component/themes/aerolite.normalcolor.xaml", UriKind.Relative)) as ResourceDictionary);
                Application.Current.MainWindow.SetTheme("Aero", new[]
                {
                    "PresentationFramework.AeroLite, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35;component/themes/aerolite.normalcolor.xaml"
                });
            }
            Application.Current.MainWindow.SetThemePrior("Aero");
        }
        public static System.Collections.IEnumerable FontSource { get; } = Fonts.SystemFontFamilies.OrderBy(x => x.FamilyNames.Values.First()).Select(x => new { Font = x, Name = x.FamilyNames.Values.Last() }).ToList();
    }
}
