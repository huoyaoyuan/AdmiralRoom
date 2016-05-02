using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Fiddler;
using Huoyaoyuan.AdmiralRoom.Officer;
using Newtonsoft.Json.Linq;

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
            date = DateTime.Today;
        }
        private DateTime? date;
        private string datastring;
        private void CompressFile()
        {
            using (var zipfile = new ZipArchive(File.Create($@"logs\battlelog\{date}.zip")))
                zipfile.CreateEntryFromFile($@"logs\battlelog\{date}.log", $"{date}.log");
            File.Delete($@"logs\battlelog\{date}.log");
        }
        private void AddApi(string type, Session oSession)
        {
            if (type == "startnext") datastring = string.Empty;
            datastring += $",\"{type}\":{{\"api\":\"{oSession.PathAndQuery.Substring(8)}\",\"data\":{oSession.GetResponseBodyAsString().Substring(7)}}}";
        }
        private void AddFleets()
        {
            datastring += ",\"fleet1\":[" + SerializeFleet(Staff.Current.Battle.SortieFleet1) + "]";
            if (Staff.Current.Battle.SortieFleet2 != null)
                datastring += ",\"fleet2\":[" + SerializeFleet(Staff.Current.Battle.SortieFleet2) + "]";
        }
        private static string SerializeFleet(Fleet fleet)
            => string.Join(",",
                fleet.Ships.Select(x =>
                {
                    var obj = new JObject(x.rawdata);
                    obj["api_slot"] = new JArray(x.Slots.Where(y => y.HasItem).Select(y => new JObject(y.Item.rawdata)).ToArray());
                    obj["api_slot_ex"] = new JObject(x.SlotEx?.Item.rawdata);
                    return obj.ToString();
                }));
        public void SetTimeStamp(DateTime utctime)
        {
            var today = utctime.Date;
            if (date != null && date != today)
            {
                CompressFile();
                date = today;
            }
            AddFleets();
            File.AppendAllLines($@"logs\battlelog\{date}.log", new[] { $"{{time:{utctime}{datastring.Replace(Environment.NewLine, string.Empty).Replace(" ", string.Empty)}}}" });
            datastring = string.Empty;
        }
    }
}
