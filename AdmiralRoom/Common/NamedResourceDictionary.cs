using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Huoyaoyuan.AdmiralRoom
{
    class NamedResourceDictionary : ResourceDictionary
    {
        public string Name { get; set; }
    }
    static class ResourceEx
    {
        public static void SetTheme(this FrameworkElement element, string themeKey, IEnumerable<string> themeSource)
        {
            element.Resources.BeginInit();
            var oldnode = element.Resources.MergedDictionaries.FirstOrDefault(d => (d as NamedResourceDictionary)?.Name == themeKey);
            var newnode = new NamedResourceDictionary { Name = themeKey };
            foreach (var source in themeSource)
                newnode.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(source, UriKind.Relative) });
            element.Resources.MergedDictionaries.Add(newnode);
            element.Resources.MergedDictionaries.Remove(oldnode);
            element.Resources.EndInit();
        }
        public static void SetThemePrior(this FrameworkElement element, string themeKey)
        {
            NamedResourceDictionary node = null;
            foreach (var d in element.Resources.MergedDictionaries)
                if ((d as NamedResourceDictionary)?.Name == themeKey)
                {
                    node = d as NamedResourceDictionary;
                    break;
                }
            if (node == null) return;
            element.Resources.MergedDictionaries.Remove(node);
            element.Resources.MergedDictionaries.Add(node);
        }
    }
}
