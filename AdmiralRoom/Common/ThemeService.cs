using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Huoyaoyuan.AdmiralRoom
{
    internal static class ThemeService
    {
        private static Dictionary<string, string[]> themes = new Dictionary<string, string[]>()
        {
            ["Office 2010 Silver"] = new[]
            {
                "pack://application:,,,/Fluent;Component/Themes/Generic.xaml",
                "pack://application:,,,/Fluent;Component/Themes/Office2010/Silver.xaml",
                "/PresentationFramework.Aero;component/themes/aero.normalcolor.xaml"
            },
            ["Office 2010 Black"] = new[]
            {
                "pack://application:,,,/Fluent;Component/Themes/Generic.xaml",
                "pack://application:,,,/Fluent;Component/Themes/Office2010/Black.xaml",
                "/PresentationFramework.Aero;component/themes/aero.normalcolor.xaml"
            },
            ["Office 2010 Blue"] = new[]
            {
                "pack://application:,,,/Fluent;Component/Themes/Generic.xaml",
                "pack://application:,,,/Fluent;Component/Themes/Office2010/Blue.xaml",
                "/PresentationFramework.Aero;component/themes/aero.normalcolor.xaml"
            },
            ["Office 2013"] = new[]
            {
                "pack://application:,,,/Fluent;Component/Themes/Office2013/Generic.xaml",
            }
        };
        public static IReadOnlyCollection<string> SupportedThemes => themes.Keys;
        public static void ChangeTheme(string theme)
        {
            try
            {
                var themeres = themes[theme];
                Application.Current.Resources.MergedDictionaries.Clear();
                foreach(var resname in themeres)
                {
                    var res = new ResourceDictionary { Source = new Uri(resname) };
                    Application.Current.Resources.MergedDictionaries.Add(res);
                }
            }
            catch { }
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
    }
}
