using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class MasterData
    {
        public MasterData()
        {
            Staff.RegisterHandler("api_start2", x => MasterHandler(x.Parse<api_start2>().Data));
        }

        public IDTable<ShipInfo> ShipInfo { get; private set; }
        public IDTable<ShipType> ShipTypes { get; private set; }
        public IDTable<EquipType> EquipTypes { get; private set; }
        public IDTable<EquipInfo> EquipInfo { get; private set; }
        void MasterHandler(api_start2 api)
        {
            ShipTypes = new IDTable<ShipType>(api.api_mst_stype.ArrayOperation(x => new ShipType(x)));
            ShipInfo = new IDTable<ShipInfo>(api.api_mst_ship.ArrayOperation(x => new ShipInfo(x)));
            EquipTypes = new IDTable<EquipType>(api.api_mst_slotitem_equiptype.ArrayOperation(x => new EquipType(x)));
            EquipInfo = new IDTable<EquipInfo>(api.api_mst_slotitem.ArrayOperation(x => new EquipInfo(x)));
        }
    }
}
