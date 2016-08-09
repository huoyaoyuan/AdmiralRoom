using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Views
{
    /// <summary>
    /// FleetView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(ISubView))]
    public partial class FleetView : UserControl, ISubView
    {
        public FleetView()
        {
            InitializeComponent();
        }

        public string ContentID => nameof(FleetView);

        public UIElement View => this;

        public string GetTitle(CultureInfo culture) => Properties.Resources.ViewTitle_FleetView;
    }
}
