using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;

#pragma warning disable CC0021

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class MasterData
    {
        public MasterData()
        {
            Staff.API("api_start2").Subscribe<api_start2>(MasterHandler);
            LoadFinalHPs();
        }

        public IDTable<MapArea> MapAreas { get; private set; }
        public IDTable<MapInfo> MapInfos { get; } = new IDTable<MapInfo>();
        public IDTable<ShipInfo> ShipInfo { get; private set; }
        public IDTable<ShipType> ShipTypes { get; private set; }
        public IDTable<EquipType> EquipTypes { get; private set; }
        public IDTable<EquipInfo> EquipInfo { get; private set; }
        public IDTable<MissionInfo> MissionInfo { get; private set; }
        private void MasterHandler(api_start2 api)
        {
            Models.Status.Current.IsGameLoaded = true;
            MapAreas = new IDTable<MapArea>(api.api_mst_maparea.Select(x => new MapArea(x)));
            MapInfos.UpdateAll(api.api_mst_mapinfo, x => x.api_id);
            ShipTypes = new IDTable<ShipType>(api.api_mst_stype.Select(x => new ShipType(x)));
            ShipInfo = new IDTable<ShipInfo>(api.api_mst_ship.Select(x => new ShipInfo(x)));
            EquipTypes = new IDTable<EquipType>(api.api_mst_slotitem_equiptype.Select(x => new EquipType(x)));
            EquipInfo = new IDTable<EquipInfo>(api.api_mst_slotitem.Select(x => new EquipInfo(x)));
            MissionInfo = new IDTable<MissionInfo>(api.api_mst_mission.Select(x => new MissionInfo(x)));
        }
        private void LoadFinalHPs()
        {
            try
            {
                using (var reader = File.OpenText(@"information\EventMapFinalHP.txt"))
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine().Split(new[] { "//" }, StringSplitOptions.None)[0];
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        var a = line.Split(':');
                        finalhptable.Add(int.Parse(a[0]), a[1].Split(',').Select(x => int.Parse(x)).ToArray());
                    }
            }
            catch { }
        }
        private readonly Dictionary<int, int[]> finalhptable = new Dictionary<int, int[]>();
        public int QueryFinalHP(MapInfo map)
        {
            try
            {
                int[] r;
                if (!finalhptable.TryGetValue(map.Id, out r)) return 1;
                if (r.Length == 1) return r[0];
                return (r[(int)map.Difficulty]);
            }
            catch { return 1; }
        }
    }
}
