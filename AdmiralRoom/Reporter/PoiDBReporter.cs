using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Huoyaoyuan.AdmiralRoom.Reporter
{
    static class PoiDBReporter
    {
        private static readonly string SERVER_HOSTNAME = "poi.0u0.moe";
        private static readonly string UAString = "AdmiralRoom Reporter v1.0";
        public static async Task ReportAsync(JObject @object, string apiname)
        {
            if (!Config.Current.ReportToPoiDB) return;
            try
            {
                var wrq = WebRequest.CreateHttp($"http://{SERVER_HOSTNAME}/api/report/v2/{apiname}");
                wrq.UserAgent = UAString;
                wrq.Method = "POST";
                var str = "data=" + @object.ToString().Replace("\r\n", "");
                wrq.ContentType = "text/plain-text";
                wrq.ContentLength = Encoding.UTF8.GetByteCount(str);
                using (var sw = new StreamWriter(await wrq.GetRequestStreamAsync()))
                {
                    sw.Write(str);
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
