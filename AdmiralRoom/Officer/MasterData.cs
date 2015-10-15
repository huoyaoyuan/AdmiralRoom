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
            ShipTypes = new IDTable<ShipType>();
            foreach(var type in api.api_mst_stype)
                ShipTypes.Add(new ShipType(type));

            ShipInfo = new IDTable<ShipInfo>();
            foreach(var ship in api.api_mst_ship)
                ShipInfo.Add(new ShipInfo(ship));

            EquipTypes = new IDTable<EquipType>();
            foreach(var type in api.api_mst_slotitem_equiptype)
                EquipTypes.Add(new EquipType(type));

            EquipInfo = new IDTable<EquipInfo>();
            foreach (var item in api.api_mst_slotitem)
                EquipInfo.Add(new EquipInfo(item));
        }
    }
}
