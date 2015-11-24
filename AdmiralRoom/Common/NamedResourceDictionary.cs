using System;
using System.Collections.Generic;
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
            NamedResourceDictionary node = null;
            foreach (var d in element.Resources.MergedDictionaries)
                if ((d as NamedResourceDictionary)?.Name == themeKey)
                {
                    node = d as NamedResourceDictionary;
                    break;
                }
            if (node == null)
            {
                node = new NamedResourceDictionary { Name = themeKey };
                element.Resources.MergedDictionaries.Add(node);
            }
            node.MergedDictionaries.Clear();
            foreach (var source in themeSource)
                node.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(source, UriKind.Relative) });
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
