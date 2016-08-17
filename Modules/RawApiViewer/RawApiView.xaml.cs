using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace RawApiViewer
{
    /// <summary>
    /// RawApiView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(ISubView))]
    public partial class RawApiView : UserControl, ISubView
    {
        public RawApiView()
        {
            InitializeComponent();
        }

        public string ContentID => "RawApiView";

        public UIElement View => this;

        public string GetTitle(CultureInfo culture) => "Raw API";
    }
}
