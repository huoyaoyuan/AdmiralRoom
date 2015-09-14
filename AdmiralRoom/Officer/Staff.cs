using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fiddler;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    class Staff
    {
        public static Staff Current { get; } = new Staff();
        public Proxy Proxy { get; set; }
        public UTF8Encoding Encoder = new UTF8Encoding();
        private Dictionary<string, Action<Session>> Handlers = new Dictionary<string, Action<Session>>();
        public void Start(int port = 39175)
        {
            FiddlerApplication.Startup(port, FiddlerCoreStartupFlags.ChainToUpstreamGateway);
            FiddlerApplication.BeforeRequest += FiddlerApplication_BeforeRequest;
            FiddlerApplication.AfterSessionComplete += AfterSessionComplete;

            Helper.RefreshIESettings($"localhost:{port}");
        }

        private void AfterSessionComplete(Session oSession)
        {
            if (oSession.PathAndQuery.StartsWith("/kcsapi") && oSession.oResponse.MIMEType.Equals("text/plain"))
            {
                Models.StatusModel.Current.StatusText = "已读取" + oSession.url;
                Thread th = new Thread(new ParameterizedThreadStart(Distribute));
                th.Start(oSession);
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
            FiddlerApplication.AfterSessionComplete -= AfterSessionComplete;
            FiddlerApplication.Shutdown();
        }

        private void Distribute(object o)
        {
            Session oSession = o as Session;
            System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
            foreach(string key in Handlers.Keys)
            {
                if (oSession.PathAndQuery.Contains(key))
                {
                    Handlers[key].BeginInvoke(oSession, null, null);
                }
            }
        }

        public void RegisterHandler(string apiname,Action<Session> handler)
        {
            Action<Session> Handler;
            Handlers.TryGetValue(apiname, out Handler);
            if (Handler == null)
            {
                Handler = new Action<Session>(handler);
                Handlers.Add(apiname, Handler);
            }
            else Handler += handler;
        }
    }
}
