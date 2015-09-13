using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiddler;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    class Staff
    {
        public static Staff Current { get; } = new Staff();
        public Proxy Proxy { get; set; }
        private UTF8Encoding Encoder = new UTF8Encoding();
        public void Start(int port = 39175)
        {
            FiddlerApplication.Startup(port, FiddlerCoreStartupFlags.ChainToUpstreamGateway);
            FiddlerApplication.BeforeRequest += FiddlerApplication_BeforeRequest;
            FiddlerApplication.AfterSessionComplete += AfterSessionComplete;

            Helper.RefreshIESettings($"localhost:{port}");
        }

        private void AfterSessionComplete(Session oSession)
        {
            if (oSession.uriContains("/kcsapi/"))
            {
                Models.StatusModel.Current.StatusText = "已读取" + oSession.url;
                Models.APIModel.Current.APIText = UnicodeEscape.Decode(Encoder.GetString(oSession.ResponseBody));
            }
        }

        private void FiddlerApplication_BeforeRequest(Session oSession)
        {
            if (Proxy != null && Config.Current.EnableProxy)
            {
                oSession["X-OverrideGateway"] = $"[{Proxy.Host}]:{Proxy.Port}";
            }
        }

        public void Stop()
        {
            FiddlerApplication.BeforeRequest -= FiddlerApplication_BeforeRequest;
            FiddlerApplication.Shutdown();
        }
    }
}
