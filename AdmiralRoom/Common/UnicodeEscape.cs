using System.Globalization;
using System.Text.RegularExpressions;

namespace Huoyaoyuan.AdmiralRoom
{
    static class UnicodeEscape
    {
        private static Regex reUnicode = new Regex(@"\\u([0-9a-fA-F]{4})", RegexOptions.Compiled);
        public static string UnicodeDecode(this string s)
        {
            return reUnicode.Replace(s, m =>
            {
                short c;
                if (short.TryParse(m.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out c))
                {
                    return "" + (char)c;
                }
                return m.Value;
            });
        }
    }
}
