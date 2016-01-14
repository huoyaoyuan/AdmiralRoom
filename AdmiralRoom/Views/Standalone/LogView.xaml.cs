using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Huoyaoyuan.AdmiralRoom.Views.Standalone
{
    /// <summary>
    /// LogView.xaml 的交互逻辑
    /// </summary>
    public partial class LogView : Window
    {
        public LogView()
        {
            InitializeComponent();
        }
        public void SetGridColumns(IEnumerable<GridViewColumn> columns) =>
            columns.ForEach(x => gridview.Columns.Add((x)));
    }
}
