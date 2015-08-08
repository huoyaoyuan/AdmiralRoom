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
        public static void EnableAero(bool Enable)
        {
            Window window = App.Current.MainWindow;
            if (Enable)
            {
                window.Resources.MergedDictionaries.Add(App.LoadComponent(new Uri("/PresentationFramework.Aero;component/themes/aero.normalcolor.xaml", UriKind.Relative)) as ResourceDictionary);
            }
            else
            {
                window.Resources.MergedDictionaries.Clear();
            }
        }
    }
}
