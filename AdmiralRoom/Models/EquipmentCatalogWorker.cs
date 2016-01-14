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
            public EquipmentImprovementGroup[] Groups { get; set; } = CollectionEx.ArrayNew<EquipmentImprovementGroup>(11);
        }
        public class EquipmentImprovementGroup
        {
            public int Level { get; set; }
            public int ImprovementType { get; set; }
            public int Count { get; set; }
            public int Left { get; set; }
            public IDictionary<Ship, int> Equipped { get; } = new Dictionary<Ship, int>();
        }
        private EquipmentCatalogWorker() { }
        public static EquipmentCatalogWorker Instance { get; } = new EquipmentCatalogWorker();

        #region Groups
        private IReadOnlyList<EquipmentGroup> _groups;
        public IReadOnlyList<EquipmentGroup> Groups
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
                int level = item.AirProficiency + item.ImproveLevel;
                EquipmentImprovementGroup group2 = group.Groups[level];
                if (level != group2.Level)
                {
                    group2.Level = level;
                    if (item.ImproveLevel > 0) group2.ImprovementType = 1;
                    else if (item.AirProficiency > 0) group2.ImprovementType = 2;
                }
                group2.Count++;
                if (item.OnShip != null)
                {
                    if (group2.Equipped.ContainsKey(item.OnShip)) group2.Equipped[item.OnShip]++;
                    else group2.Equipped.Add(item.OnShip, 1);
                }
                else group2.Left++;
            }
            d.Values.ForEach(x => x.Groups = x.Groups.Where(y => y.Count > 0).ToArray());
            Groups = d.Values.OrderBy(x => x.Item.EquipType.Id).ThenBy(x => x.Item.Id).ToArray();
        }
    }
}
