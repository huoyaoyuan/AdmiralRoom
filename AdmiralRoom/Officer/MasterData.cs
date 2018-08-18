using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fiddler;
using Huoyaoyuan.AdmiralRoom.API;
using Meowtrix.Collections.Generic;
using Meowtrix.Text;

#pragma warning disable CC0021

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class MasterData
    {
        public MasterData()
        {
            Staff.API("api_start2/getData").Subscribe<api_start2>(MasterHandler);
            Staff.API("api_start2/getData").Subscribe(Save);
            Staff.API("api_start2/getData").Subscribe((Session x) => Models.Status.Current.IsGameLoaded = true);
            LoadFinalHPs();
        }

        public IDTable<int, MapArea> MapAreas { get; private set; }
        public IDTable<int, MapInfo> MapInfos { get; } = new IDTable<int, MapInfo>();
        public IDTable<int, ShipInfo> ShipInfo { get; private set; }
        public IDTable<int, ShipType> ShipTypes { get; private set; }
        public IDTable<int, EquipType> EquipTypes { get; private set; }
        public IDTable<int, EquipInfo> EquipInfo { get; private set; }
        public IDTable<int, MissionInfo> MissionInfo { get; private set; }
        public IDTable<int, UseItem> UseItems { get; private set; }
        private void MasterHandler(api_start2 api)
        {
            MapAreas = new IDTable<int, MapArea>(api.api_mst_maparea.Select(x => new MapArea(x)));
            MapInfos.UpdateAll(api.api_mst_mapinfo, x => x.api_id);
            ShipTypes = new IDTable<int, ShipType>(api.api_mst_stype.Select(x => new ShipType(x)));
            ShipInfo = new IDTable<int, ShipInfo>(api.api_mst_ship.Select(x => new ShipInfo(x)));
            EquipTypes = new IDTable<int, EquipType>(api.api_mst_slotitem_equiptype.Select(x => new EquipType(x)));
            EquipInfo = new IDTable<int, EquipInfo>(api.api_mst_slotitem.Select(x => new EquipInfo(x)));
            MissionInfo = new IDTable<int, MissionInfo>(api.api_mst_mission.Select(x => new MissionInfo(x)));
            UseItems = new IDTable<int, UseItem>(api.api_mst_useitem.Select(x => new UseItem(x)));
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
                        finalhptable.Add(int.Parse(a[0]), a[1].Split(',').Select(int.Parse).ToArray());
                    }
            }
            catch { }
        }
        private readonly Dictionary<int, int[]> finalhptable = new Dictionary<int, int[]>();
        public int QueryFinalHP(MapInfo map)
        {
            try
            {
                if (!finalhptable.TryGetValue(map.Id, out var r)) return 1;
                if (r.Length == 1) return r[0];
                return (r[(int)map.Difficulty]);
            }
            catch { return 1; }
        }
        public void Load()
        {
            try
            {
                using (var reader = File.OpenText(@"information\masterdata.json"))
                {
                    MasterHandler(APIHelper.Parse<api_start2>(reader).api_data);
                }
            }
            catch { }
        }
        public void Save(CachedSession oSession)
        {
            try
            {
                Directory.CreateDirectory("logs");
                using (var writer = File.CreateText(@"information\masterdata.json"))
                {
                    writer.Write(oSession.JsonResponse.UnicodeDecode());
                    writer.Flush();
                }
            }
            catch { }
        }
    }
}
