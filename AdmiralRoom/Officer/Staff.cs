using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using Fiddler;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    class Staff
    {
        private static Staff _current;
        public static Staff Current
        {
            get
            {
                if (_current == null) _current = new Staff();
                return _current;
            }
        }
        public Dispatcher Dispatcher { get; set; }
        public static Proxy Proxy { get; set; }
        public static UTF8Encoding Encoder = new UTF8Encoding();
        private static Dictionary<string, Action<Session>> Handlers = new Dictionary<string, Action<Session>>();
        public static void Start(int port = 39175)
        {
            FiddlerApplication.Startup(port, FiddlerCoreStartupFlags.ChainToUpstreamGateway);
            FiddlerApplication.BeforeRequest += FiddlerApplication_BeforeRequest;
            FiddlerApplication.AfterSessionComplete += AfterSessionComplete;

            Helper.RefreshIESettings($"localhost:{port}");
        }

        private static void AfterSessionComplete(Session oSession)
        {
            if (oSession.PathAndQuery.StartsWith("/kcsapi") && oSession.oResponse.MIMEType.Equals("text/plain"))
            {
                Models.StatusModel.Current.StatusText = "已读取" + oSession.url;
                Thread th = new Thread(new ParameterizedThreadStart(Distribute));
                th.Start(oSession);
            }
        }

        private static void FiddlerApplication_BeforeRequest(Session oSession)
        {
            if (Proxy != null && Config.Current.EnableProxy)
            {
                oSession["X-OverrideGateway"] = $"[{Proxy.Host}]:{Proxy.Port}";
            }
        }

        public static void Stop()
        {
            FiddlerApplication.BeforeRequest -= FiddlerApplication_BeforeRequest;
            FiddlerApplication.AfterSessionComplete -= AfterSessionComplete;
            FiddlerApplication.Shutdown();
        }

        private static void Distribute(object o)
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

        public static void RegisterHandler(string apiname,Action<Session> handler)
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
        public Admiral Admiral { get; } = new Admiral();
        public Homeport Homeport { get; } = new Homeport();
        public MasterData MasterData { get; } = new MasterData();
    }
}
