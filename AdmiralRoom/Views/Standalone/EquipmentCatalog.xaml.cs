using System.Windows;

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
}
