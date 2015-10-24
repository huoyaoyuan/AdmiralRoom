using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class MasterData
    {
        public MasterData()
        {
            Staff.API("api_start2").Subscribe<api_start2>(MasterHandler);
        }

        public IDTable<MapArea> MapAreas { get; private set; }
        public IDTable<MapInfo> MapInfos { get; private set; }
        public IDTable<ShipInfo> ShipInfo { get; private set; }
        public IDTable<ShipType> ShipTypes { get; private set; }
        public IDTable<EquipType> EquipTypes { get; private set; }
        public IDTable<EquipInfo> EquipInfo { get; private set; }
        public IDTable<MissionInfo> MissionInfo { get; private set; }
        void MasterHandler(api_start2 api)
        {
            Models.Status.Current.IsGameLoaded = true;
            MapAreas = new IDTable<MapArea>(api.api_mst_maparea.ArrayOperation(x => new MapArea(x)));
            MapInfos = new IDTable<MapInfo>(api.api_mst_mapinfo.ArrayOperation(x => new MapInfo(x)));
            ShipTypes = new IDTable<ShipType>(api.api_mst_stype.ArrayOperation(x => new ShipType(x)));
            ShipInfo = new IDTable<ShipInfo>(api.api_mst_ship.ArrayOperation(x => new ShipInfo(x)));
            EquipTypes = new IDTable<EquipType>(api.api_mst_slotitem_equiptype.ArrayOperation(x => new EquipType(x)));
            EquipInfo = new IDTable<EquipInfo>(api.api_mst_slotitem.ArrayOperation(x => new EquipInfo(x)));
            MissionInfo = new IDTable<MissionInfo>(api.api_mst_mission.ArrayOperation(x => new MissionInfo(x)));
        }
    }
}
