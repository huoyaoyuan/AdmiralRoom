using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Views
{
    /// <summary>
    /// BuildingView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(ISubView))]
    public partial class BuildingView : UserControl, ISubView
    {
        public BuildingView()
        {
            InitializeComponent();
        }

        public string ContentID => nameof(BuildingView);

        public UIElement View => this;

        public string GetTitle(CultureInfo culture) => StringTable.ViewTitle_BuildingView;
    }
}
