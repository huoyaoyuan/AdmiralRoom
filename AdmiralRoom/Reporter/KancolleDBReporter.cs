using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Fiddler;

namespace Huoyaoyuan.AdmiralRoom.Reporter
{
    static class KancolleDBReporter
    {
        public static void Initialize() => FiddlerApplication.AfterSessionComplete += ReportAsync;
        private static readonly string[] apinames =
        {
            "api_port/port",
            "api_get_member/ship2",
            "api_get_member/ship3",
            "api_get_member/slot_item",
            "api_get_member/kdock",
            "api_get_member/mapinfo",
            "api_req_hensei/change",
            "api_req_kousyou/createship",
            "api_req_kousyou/getship",
            "api_req_kousyou/createitem",
            "api_req_map/start",
            "api_req_map/next",
            "api_req_map/select_eventmap_rank",
            "api_req_sortie/battle",
            "api_req_battle_midnight/battle",
            "api_req_battle_midnight/sp_midnight",
            "api_req_sortie/night_to_day",
            "api_req_sortie/battleresult",
            "api_req_combined_battle/battle",
            "api_req_combined_battle/airbattle",
            "api_req_combined_battle/midnight_battle",
            "api_req_combined_battle/battleresult",
            "api_req_sortie/airbattle",
            "api_req_combined_battle/battle_water",
            "api_req_combined_battle/sp_midnight"
        };
        private static async void ReportAsync(Session oSession)
        {
            if (!Config.Current.ReportToKancolleDB) return;
            foreach (var api in apinames)
                if (oSession.PathAndQuery.Contains(api))
                {
                    try
                    {
                        var request = HttpUtility.HtmlDecode(oSession.GetRequestBodyAsString());
                        request = Regex.Replace(request, @"&api(_|%5F)token=[0-9a-f]+|api(_|%5F)token=[0-9a-f]+&?", "");
                        var response = oSession.GetResponseBodyAsString().Replace("svdata=", "");
                        var wrq = WebRequest.CreateHttp("http://api.kancolle-db.net/2/");
                        wrq.Method = "POST";
                        wrq.ContentType = "application/x-www-form-urlencoded";
                        var data = "token=" + HttpUtility.UrlEncode(Config.Current.KancolleDBToken)
                            + "&agent=LZXNXVGPejgSnEXLH2ur"//伪装为KCV
                            + "&url=" + HttpUtility.UrlEncode(oSession.fullUrl)
                            + "&requestbody=" + HttpUtility.UrlEncode(request)
                            + "&responsebody=" + HttpUtility.UrlEncode(response);
                        wrq.ContentLength = Encoding.UTF8.GetByteCount(data);
                        using (var sw = new StreamWriter(await wrq.GetRequestStreamAsync()))
                        {
                            sw.Write(data);
                            sw.Flush();
                        }
                        using (var wrs = wrq.GetResponse() as HttpWebResponse)
                        {
                            System.Diagnostics.Debug.WriteLine(wrs.StatusCode);
                        }
                    }
                    catch (WebException ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                }
        }
    }
}
