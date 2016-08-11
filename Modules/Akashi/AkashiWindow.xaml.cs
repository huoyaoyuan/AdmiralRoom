using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Modules.Akashi
{
    /// <summary>
    /// AkashiWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AkashiWindow : Window
    {
        public AkashiWindow()
        {
            InitializeComponent();
        }
    }
    [Export(typeof(ISubWindow))]
    public class AkashiSubWindow : ISubWindow
    {
        public SubWindowCategory Category => SubWindowCategory.Information;

        public Window CreateWindow() => new AkashiWindow();

        public string GetTitle(CultureInfo culture)
        {
            switch (culture.Name.ToLowerInvariant())
            {
                case "en":
                    return "Improvement Arsenal";
                case "zh-hans":
                    return "改修工厂";
                default:
                    return "改修工廠";
            }
        }
    }
}
