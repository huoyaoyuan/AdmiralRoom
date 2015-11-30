using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Huoyaoyuan.AdmiralRoom.Views
{
    /// <summary>
    /// DevelopView.xaml 的交互逻辑
    /// </summary>
    public partial class DevelopView : UserControl
    {
        public DevelopView()
        {
            InitializeComponent();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            (content.ItemsSource as IList).Clear();
        }
    }
}
