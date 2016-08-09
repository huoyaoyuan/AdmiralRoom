using System.Globalization;
using System.Windows;

namespace Huoyaoyuan.AdmiralRoom.Composition
{
    public interface ISubView
    {
        UIElement View { get; }
        string ContentID { get; }
        string GetTitle(CultureInfo culture);
    }
}
