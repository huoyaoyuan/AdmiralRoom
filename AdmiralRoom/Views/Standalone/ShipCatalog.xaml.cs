using System.Windows;
using System.Windows.Controls;

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
        private readonly Models.ShipCatalogWorker worker = Models.ShipCatalogWorker.Instance;
        public void SelectTypesCommand(object sender, RoutedEventArgs e)
        {
            var param = (sender as Button).Tag as int[];
            worker.SelectTypes(param);
        }
    }
}
