using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class EquipType : GameObject<api_mst_slotitem_equiptype>
    {
        public override int Id => rawdata.api_id;
        public string Name => rawdata.api_name;
        private static readonly int[] Planes = { 6, 7, 8, 9, 10, 11, 41 };
        public bool IsAirPlane => Planes.Contains(Id);
        public EquipType() { }
        public EquipType(api_mst_slotitem_equiptype api) : base(api) { }
    }
}
