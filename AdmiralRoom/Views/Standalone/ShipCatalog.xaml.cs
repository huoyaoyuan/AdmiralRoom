using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Huoyaoyuan.AdmiralRoom.Views.Standalone
{
    /// <summary>
    /// AllShips.xaml 的交互逻辑
    /// </summary>
    public partial class ShipCatalog : Window
    {
        public ShipCatalog()
        {
            InitializeComponent();
            worker.Initialize();
            worker.Update();
            this.DataContext = worker;
        }
        Models.ShipCatalogWorker worker = Models.ShipCatalogWorker.Instance;
        public void SelectTypesCommand(object sender, RoutedEventArgs e)
        {
            var param = (sender as Button).Tag as int[];
            worker.SelectTypes(param);
        }
    }
}
