using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Views
{
    /// <summary>
    /// MapHPView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(ISubView))]
    public partial class MapHPView : UserControl, ISubView
    {
        public MapHPView()
        {
            InitializeComponent();
        }

        public string ContentID => nameof(MapHPView);

        public UIElement View => this;

        public string GetTitle(CultureInfo culture) => StringTable.ViewTitle_MapHPView;
    }
}
