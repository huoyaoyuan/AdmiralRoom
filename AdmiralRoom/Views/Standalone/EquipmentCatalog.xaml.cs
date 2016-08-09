using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Views.Standalone
{
    /// <summary>
    /// EquipmentCatalog.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentCatalog : Window
    {
        public EquipmentCatalog()
        {
            InitializeComponent();
            worker.Update();
            this.DataContext = worker;
        }
        private readonly Models.EquipmentCatalogWorker worker = Models.EquipmentCatalogWorker.Instance;
    }

    [Export(typeof(ISubWindow))]
    public class EquipmentCatalogSubWindow : ISubWindow
    {
        public Window CreateWindow() => new EquipmentCatalog();

        public string GetTitle(CultureInfo culture) => Properties.Resources.ViewTitle_EquipmentCatalog;
    }
}
