using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class EquipType : GameObject<api_mst_slotitem_equiptype>, IIdentifiable
    {
        public int Id => rawdata.api_id;
        public string Name => rawdata.api_name ?? "？？？";
        public EquipType(api_mst_slotitem_equiptype api) : base(api) { }
    }
}
