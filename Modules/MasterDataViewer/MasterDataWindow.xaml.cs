using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Modules.MasterDataViewer
{
    /// <summary>
    /// MasterDataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MasterDataWindow : Window
    {
        public MasterDataWindow()
        {
            InitializeComponent();
        }
    }
    [Export(typeof(ISubWindow))]
    public class MasterDataSubWindow : ISubWindow
    {
        public SubWindowCategory Category => SubWindowCategory.Information;

        public Window CreateWindow() => new MasterDataWindow();

        public string GetTitle(CultureInfo culture) => "MasterData";
    }
}
