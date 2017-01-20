using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class EquipType : GameObject<api_mst_slotitem_equiptype>
    {
        public override int Id => rawdata.api_id;
        public string Name => rawdata.api_name;
        private static readonly int[] planes = { 6, 7, 8, 9, 10, 11, 41, 45, 56, 57, 58 };
        public bool IsAirPlane => planes.Contains(Id);
        public EquipType(api_mst_slotitem_equiptype api) : base(api) { }
    }
}
