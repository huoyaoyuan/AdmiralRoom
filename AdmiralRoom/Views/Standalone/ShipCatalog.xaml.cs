using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Huoyaoyuan.AdmiralRoom.Composition;

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

    [Export(typeof(ISubWindow))]
    public class ShipCatalogSubWindow : ISubWindow
    {
        public Window CreateWindow() => new ShipCatalog();

        public string GetTitle(CultureInfo culture) => Properties.Resources.ViewTitle_ShipCatalog;

        public SubWindowCategory Category => SubWindowCategory.Overview;
    }
}
