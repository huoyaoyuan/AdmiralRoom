using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Huoyaoyuan.AdmiralRoom.Officer;
using Meowtrix.Collections.Generic;
using Newtonsoft.Json;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    class BattleDetailLogger
    {
        public BattleDetailLogger()
        {
            Directory.CreateDirectory(@"logs\battlelog");
            Staff.API("api_req_map/start").Subscribe(x => AddApi("startnext", x));
            Staff.API("api_req_map/next").Subscribe(x => AddApi("startnext", x));
            Staff.API("api_req_sortie/battleresult").Subscribe(x => AddApi("battleresult", x));
            Staff.API("api_req_combined_battle/battleresult").Subscribe(x => AddApi("battleresult", x));
            Staff.API("api_req_sortie/battle").Subscribe(x => AddApi("battle", x));
            Staff.API("api_req_battle_midnight/battle").Subscribe(x => AddApi("nightbattle", x));
            Staff.API("api_req_battle_midnight/sp_midnight").Subscribe(x => AddApi("battle", x));
            //Staff.API("api_req_practice/battle").Subscribe(x => AddApi("battle", x));
            //Staff.API("api_req_practice/midnight_battle").Subscribe(x => AddApi("nightbattle", x));
            Staff.API("api_req_sortie/airbattle").Subscribe(x => AddApi("battle", x));
            Staff.API("api_req_sortie/ld_airbattle").Subscribe(x => AddApi("battle", x));
            Staff.API("api_req_combined_battle/airbattle").Subscribe(x => AddApi("battle", x));
            Staff.API("api_req_combined_battle/battle").Subscribe(x => AddApi("battle", x));
            Staff.API("api_req_combined_battle/midnight_battle").Subscribe(x => AddApi("nightbattle", x));
            Staff.API("api_req_combined_battle/sp_midnight").Subscribe(x => AddApi("battle", x));
            Staff.API("api_req_combined_battle/battle_water").Subscribe(x => AddApi("battle", x));
            Staff.API("api_req_combined_battle/ld_airbattle").Subscribe(x => AddApi("battle", x));
            Staff.API("api_req_combined_battle/ec_battle").Subscribe(x => AddApi("battle", x));
            Staff.API("api_req_combined_battle/ec_midnight_battle").Subscribe(x => AddApi("nightbattle", x));
            Staff.API("api_req_combined_battle/each_battle").Subscribe(x => AddApi("battle", x));
            Staff.API("api_req_combined_battle/each_battle_water").Subscribe(x => AddApi("battle", x));
            Staff.API("api_req_combined_battle/ec_night_to_day").Subscribe(x => AddApi("battle", x));
            Staff.API("api_req_sortie/ld_shooting").Subscribe(x => AddApi("battle", x));
            Staff.API("api_req_combined_battle/ld_shooting").Subscribe(x => AddApi("battle", x));
            var dir = new DirectoryInfo(@"logs\battlelog");
            foreach (var file in dir.GetFiles("*.log"))
            {
                date = DateTime.Parse(Path.GetFileNameWithoutExtension(file.Name));
                if (date != DateTime.UtcNow.Date)
                    CompressFile();
            }
            date = DateTime.UtcNow.Date;
        }
        private DateTime? date;
        private string datastring;
        private void CompressFile()
        {
            if (File.Exists($@"logs\battlelog\{date:yyyy-MM-dd}.log"))
            {
                using (var zipfile = new ZipArchive(File.Create($@"logs\battlelog\{date:yyyy-MM-dd}.zip"), ZipArchiveMode.Create))
                    zipfile.CreateEntryFromFile($@"logs\battlelog\{date:yyyy-MM-dd}.log", $"{date:yyyy-MM-dd}.log");
                File.Delete($@"logs\battlelog\{date:yyyy-MM-dd}.log");
            }
        }
        private void AddApi(string type, CachedSession oSession)
        {
            if (type == "startnext") datastring = string.Empty;
            datastring += $",\"{type}\":{{\"api\":\"{oSession.Session.PathAndQuery.Substring(8)}\",\"data\":{oSession.JsonResponse}}}";
        }
        private void AddFleets()
        {
            datastring += ",\"fleet1\":[" + SerializeFleet(Staff.Current.Battle.SortieFleet1) + "]";
            if (Staff.Current.Battle.SortieFleet2 != null)
                datastring += ",\"fleet2\":[" + SerializeFleet(Staff.Current.Battle.SortieFleet2) + "]";
        }
        private static string SerializeFleet(Fleet fleet)
            => string.Join(",",
                fleet.Ships.Select(x => string.Concat(
                        "{\"id\":",
                        x.Id,
                        ",\"shipid\":",
                        x.ShipId,
                        ",\"lv\":",
                        x.Level,
                        ",\"karyoku\":",
                        x.Firepower.Current,
                        ",\"raisou\":",
                        x.Torpedo.Current,
                        ",\"taiku\":",
                        x.AA.Current,
                        ",\"soukou\":",
                        x.Armor.Current,
                        ",\"kaihi\":",
                        x.Evasion.Current,
                        ",\"taisen\":",
                        x.ASW.Current,
                        ",\"sakuteki\":",
                        x.LoS.Current,
                        ",\"lucky\":",
                        x.Luck.Current,
                        ",\"slots\":[",
                        string.Join(",", x.Slots.Where(y => y.HasItem).Select(y => EquipToString(y.Item))),
                        "],\"slotex\":",
                        x.SlotEx.HasItem ? EquipToString(x.SlotEx.Item) : "null",
                        "}")
                ));
        private static string EquipToString(Equipment equip)
            => string.Concat(
                "{\"itemid\":",
                equip.EquipInfo.Id,
                ",\"level\":",
                equip.ImproveLevel,
                ",\"alv\":",
                equip.AirProficiency,
                "}");
        public void SetTimeStamp(DateTime utctime)
        {
            var today = utctime.Date;
            if (date != null && date != today)
            {
                CompressFile();
                date = today;
            }
            AddFleets();
            File.AppendAllLines($@"logs\battlelog\{date:yyyy-MM-dd}.log", new[] { $"{{\"time\":\"{utctime:yyyy/M/dd H:mm:ss}\"{datastring.Replace(Environment.NewLine, string.Empty)}}}" });
            datastring = string.Empty;
        }

        private static readonly JsonSerializerSettings JSettings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Error = (_, e) => e.ErrorContext.Handled = true
        };
        private static readonly DateTime battleApiChangeStamp = new DateTime(2017, 11, 17, 3, 0, 0);
        private const int cacheDays = 5;
        private List<DateTime> cacheIndex = new List<DateTime>();
        private List<IDTable<DateTime, IBattleDetail>> cacheList = new List<IDTable<DateTime, IBattleDetail>>();
        public IBattleDetail FindLog(DateTime utctime)
        {
            var date = utctime.Date;
            bool old = utctime < battleApiChangeStamp;
            int index = cacheIndex.IndexOf(date);
            if (index == -1)
            {
                if (cacheIndex.Count >= cacheDays)
                {
                    cacheIndex.RemoveAt(0);
                    cacheList.RemoveAt(0);
                }
                if (File.Exists($@"logs\battlelog\{date:yyyy-MM-dd}.log"))
                {
                    foreach (string line in File.ReadAllLines($@"logs\battlelog\{date:yyyy-MM-dd}.log"))
                    {
                        IBattleDetail item;
                        if (old)
                            item = JsonConvert.DeserializeObject<OldBattleDetail>(line, JSettings);
                        else item = JsonConvert.DeserializeObject<BattleDetail>(line, JSettings);
                        if (item.GetTimeStamp() == utctime)
                            return item;
                    }
                    return null;
                }
                else if (File.Exists($@"logs\battlelog\{date:yyyy-MM-dd}.zip"))
                {
                    var cache = new IDTable<DateTime, IBattleDetail>();
                    using (var zipfile = ZipFile.OpenRead($@"logs\battlelog\{date:yyyy-MM-dd}.zip"))
                    using (var stream = zipfile.GetEntry($"{date:yyyy-MM-dd}.log").Open())
                    {
                        var reader = new StreamReader(stream);
                        while (!reader.EndOfStream)
                            if (old)
                                cache.Add(JsonConvert.DeserializeObject<OldBattleDetail>(FixBrokenLog(reader.ReadLine()), JSettings));
                            else cache.Add(JsonConvert.DeserializeObject<BattleDetail>(FixBrokenLog(reader.ReadLine()), JSettings));
                    }
                    index = cacheList.Count;
                    cacheIndex.Add(date);
                    cacheList.Add(cache);
                }
                else return null;
            }
            return cacheList[index][utctime] ?? cacheList[index][utctime.AddHours(-12)];
        }
        private static string FixBrokenLog(string input)
        {
            int first = input.IndexOf("\"battle\"");
            int last = input.LastIndexOf("\"battle\"");
            if (first != last)
                return input.Insert(last + 1, "night");
            else return input;
        }
    }
}
