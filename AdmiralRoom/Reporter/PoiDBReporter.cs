using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Huoyaoyuan.AdmiralRoom.Reporter
{
    static class PoiDBReporter
    {
        private static readonly string SERVER_HOSTNAME = "poi.0u0.moe";
        private static readonly string UAString = "AdmiralRoom Reporter v1.0";
        public static async void ReportAsync(JObject @object, string apiname)
        {
            var wrq = WebRequest.CreateHttp($"http://{SERVER_HOSTNAME}/api/report/v2/{apiname}");
            wrq.UserAgent = UAString;
            wrq.Method = "POST";
            var str = "data=" + @object.ToString().Replace("\r\n", "");
            wrq.ContentLength = Encoding.UTF8.GetByteCount(str);
            using (var rqs = wrq.GetRequestStream())
            using (var sw = new StreamWriter(rqs))
            {
                sw.Write(str);
                sw.Flush();
            }
            wrq.ContentType = "text/plain-text";
            try
            {
                using (var wrs = await wrq.GetResponseAsync() as HttpWebResponse)
                {
                    System.Diagnostics.Debug.WriteLine(wrs.StatusCode);
                }
            }
            catch { }
        }
    }
}
