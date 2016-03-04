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
            public int Count => Groups.Sum(x => x.Count);
        }
        public class EquipmentImprovementGroup
        {
            public int Level { get; set; }
            public int Count => Groups.Sum(x => x.Count);
            public List<EquipmentProficiencyGroup> Groups { get; } = new List<EquipmentProficiencyGroup>();
        }
        public class EquipmentProficiencyGroup
        {
            public int Level { get; set; }
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
                int i;
                EquipmentImprovementGroup group2 = null;
                for (i = 0; i < group.Groups.Count; i++)
                {
                    if (group.Groups[i].Level == item.ImproveLevel) group2 = group.Groups[i];
                    if (group.Groups[i].Level >= item.ImproveLevel) break;
                }
                if (group2 == null)
                {
                    group2 = new EquipmentImprovementGroup { Level = item.ImproveLevel };
                    group.Groups.Insert(i, group2);
                }
                EquipmentProficiencyGroup group3 = null;
                for (i = 0; i < group2.Groups.Count; i++)
                {
                    if (group2.Groups[i].Level == item.AirProficiency) group3 = group2.Groups[i];
                    if (group2.Groups[i].Level >= item.AirProficiency) break;
                }
                if (group3 == null)
                {
                    group3 = new EquipmentProficiencyGroup { Level = item.AirProficiency };
                    group2.Groups.Insert(i, group3);
                }

                group3.Count++;
                if (item.OnShip != null)
                {
                    if (group3.Equipped.ContainsKey(item.OnShip)) group3.Equipped[item.OnShip]++;
                    else group3.Equipped.Add(item.OnShip, 1);
                }
                else group3.Left++;
            }
            Groups = d.Values.OrderBy(x => x.Item.EquipType.Id).ThenBy(x => x.Item.Id).ToArray();
        }
    }
}
