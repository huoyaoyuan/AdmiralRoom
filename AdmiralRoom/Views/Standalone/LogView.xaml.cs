using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Huoyaoyuan.AdmiralRoom.Logger;

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
            DataContextChanged += (_, e) =>
            {
                gridview.Columns.Clear();
                SetGridColumns(((ViewProviderBase)e.NewValue).ViewColumns);
            };
        }
        public void SetGridColumns(IEnumerable<GridViewColumn> columns) =>
            columns.ForEach(x => gridview.Columns.Add((x)));
        public string TitleKey
        {
            set
            {
                ResourceService.SetStringTableReference(this, Window.TitleProperty, value);
            }
        }
    }
}
