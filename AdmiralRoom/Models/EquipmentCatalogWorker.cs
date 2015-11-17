using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Huoyaoyuan.AdmiralRoom.Officer;

namespace Huoyaoyuan.AdmiralRoom.Models
{
    class EquipmentCatalogWorker : NotificationObject
    {
        public class EquipmentGroup
        {
            public EquipInfo Item { get; set; }
            public int Left { get; set; }
            public List<Equipment> Items { get; set; }
        }
        private EquipmentCatalogWorker() { }
        public static EquipmentCatalogWorker Instance { get; } = new EquipmentCatalogWorker();

        #region Groups
        private IReadOnlyCollection<EquipmentGroup> _groups;
        public IReadOnlyCollection<EquipmentGroup> Groups
        {
            get { return _groups; }
            set
            {
                if (_groups != value)
                {
                    _groups = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public void Update()
        {
            var d = new Dictionary<int, EquipmentGroup>();
            if (Staff.Current.Homeport.Equipments == null) return;
            foreach (var item in Staff.Current.Homeport.Equipments)
            {
                int typeid = item.EquipInfo.Id;
                EquipmentGroup group;
                if (!d.TryGetValue(typeid, out group))
                {
                    group = new EquipmentGroup { Item = item.EquipInfo, Items = new List<Equipment>() };
                    d.Add(typeid, group);
                }
                group.Items.Add(item);
                if (item.OnShip == null) group.Left++;
            }
            Groups = d.Values.ToArray();
        }
    }
}
