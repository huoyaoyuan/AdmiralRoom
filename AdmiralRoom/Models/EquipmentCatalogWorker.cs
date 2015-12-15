using System.Collections.Generic;
using System.Linq;
using Huoyaoyuan.AdmiralRoom.Officer;

namespace Huoyaoyuan.AdmiralRoom.Models
{
    class EquipmentCatalogWorker : NotificationObject
    {
        public class EquipmentGroup
        {
            public EquipInfo Item { get; set; }
            public List<EquipmentImprovementGroup> Groups { get; } = new List<EquipmentImprovementGroup>();
        }
        public class EquipmentImprovementGroup
        {
            public int Level { get; set; }
            public int ImprovementType { get; set; }
            public int Count { get; set; }
            public int Left { get; set; }
            public Dictionary<Ship, int> Equipped { get; } = new Dictionary<Ship, int>();
        }
        private EquipmentCatalogWorker() { }
        public static EquipmentCatalogWorker Instance { get; } = new EquipmentCatalogWorker();

        #region Groups
        private IEnumerable<EquipmentGroup> _groups;
        public IEnumerable<EquipmentGroup> Groups
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
                    group = new EquipmentGroup { Item = item.EquipInfo };
                    d.Add(typeid, group);
                }
                EquipmentImprovementGroup group2 = null;
                int i;
                for (i = 0; i < group.Groups.Count; i++)
                    if (group.Groups[i].Level >= item.AirProficiency + item.ImproveLevel)
                    {
                        if (group.Groups[i].Level == item.AirProficiency + item.ImproveLevel) group2 = group.Groups[i];
                        break;
                    }
                if (group2 == null)
                {
                    group2 = new EquipmentImprovementGroup { Level = item.AirProficiency + item.ImproveLevel };
                    if (item.ImproveLevel > 0) group2.ImprovementType = 1;
                    else if (item.AirProficiency > 0) group2.ImprovementType = 2;
                    group.Groups.Insert(i, group2);
                }
                group2.Count++;
                if (item.OnShip != null)
                {
                    if (group2.Equipped.ContainsKey(item.OnShip)) group2.Equipped[item.OnShip]++;
                    else group2.Equipped.Add(item.OnShip, 1);
                }
                else group2.Left++;
            }
            Groups = d.Values.OrderBy(x => x.Item.EquipType.Id).ThenBy(x => x.Item.Id).ToArray();
        }
    }
}
