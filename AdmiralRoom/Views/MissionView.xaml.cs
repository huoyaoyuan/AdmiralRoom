using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Views
{
    /// <summary>
    /// MissionView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(ISubView))]
    public partial class MissionView : UserControl, ISubView
    {
        public MissionView()
        {
            InitializeComponent();
        }

        public string ContentID => nameof(MissionView);

        public UIElement View => this;

        public string GetTitle(CultureInfo culture) => StringTable.ViewTitle_MissionView;
    }
}
