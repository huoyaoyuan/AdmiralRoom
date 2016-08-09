using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Views
{
    /// <summary>
    /// AdmiralView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(ISubView))]
    public partial class AdmiralView : UserControl, ISubView
    {
        public AdmiralView()
        {
            InitializeComponent();
        }

        public string ContentID => nameof(AdmiralView);

        public UIElement View => this;

        public string GetTitle(CultureInfo culture) => Properties.Resources.ViewTitle_AdmiralView;
    }
}
