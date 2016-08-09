using System.Collections.Generic;
using System.Linq;
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
    }
}
