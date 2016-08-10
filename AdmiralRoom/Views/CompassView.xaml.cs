using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Views
{
    /// <summary>
    /// CompassView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(ISubView))]
    public partial class CompassView : UserControl, ISubView
    {
        public CompassView()
        {
            InitializeComponent();
        }

        public string ContentID => nameof(CompassView);

        public UIElement View => this;

        public string GetTitle(CultureInfo culture) => StringTable.ViewTitle_CompassView;
    }
}
