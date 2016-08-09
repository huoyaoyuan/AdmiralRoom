using System.Globalization;
using System.Windows;

namespace Huoyaoyuan.AdmiralRoom.Composition
{
    public interface ISubWindow
    {
        Window CreateWindow();
        string GetTitle(CultureInfo culture);
    }
}
