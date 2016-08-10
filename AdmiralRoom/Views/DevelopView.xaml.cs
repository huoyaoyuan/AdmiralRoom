using System.Collections;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Views
{
    /// <summary>
    /// DevelopView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(ISubView))]
    public partial class DevelopView : UserControl, ISubView
    {
        public DevelopView()
        {
            InitializeComponent();
        }

        public string ContentID => nameof(DevelopView);

        public UIElement View => this;

        public string GetTitle(CultureInfo culture) => StringTable.ViewTitle_DevelopView;

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            (content.ItemsSource as IList).Clear();
        }
    }
}
