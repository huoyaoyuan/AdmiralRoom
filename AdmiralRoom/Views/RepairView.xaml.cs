using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Views
{
    /// <summary>
    /// RepairView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(ISubView))]
    public partial class RepairView : UserControl, ISubView
    {
        public RepairView()
        {
            InitializeComponent();
        }

        public string ContentID => nameof(RepairView);

        public UIElement View => this;

        public string GetTitle(CultureInfo culture) => Properties.Resources.ViewTitle_RepairView;
    }
}
