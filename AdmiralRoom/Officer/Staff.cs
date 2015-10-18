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
            FiddlerApplication.BeforeRequest += SetSessionProxy;
            FiddlerApplication.AfterSessionComplete += AfterSessionComplete;

            Helper.RefreshIESettings($"localhost:{port}");
        }

        private static void AfterSessionComplete(Session oSession)
        {
            if (oSession.PathAndQuery.StartsWith("/kcsapi") && oSession.oResponse.MIMEType.Equals("text/plain"))
            {
                Models.StatusModel.Current.StatusText = "已读取" + oSession.url;
                //Thread th = new Thread(new ParameterizedThreadStart(Distribute));
                //th.Start(oSession);
                Distribute(oSession);
            }
        }

        private static void SetSessionProxy(Session oSession)
        {
            if (IsSessionHTTPS(oSession))
            {
                if (Config.Current.EnableProxyHTTPS && Config.Current.HTTPSProxy != null)
                {
                    oSession["X-OverrideGateway"] = $"[{Config.Current.HTTPSProxy.Host}]:{Config.Current.HTTPSProxy.Port}";
                    return;
                }
            }
            if (Proxy != null && Config.Current.EnableProxy)
            {
                oSession["X-OverrideGateway"] = $"[{Proxy.Host}]:{Proxy.Port}";
            }
        }

        public static bool IsSessionHTTPS(Session oSession)
        {
            if (oSession.isHTTPS) return true;
            if (oSession.url.Contains(":443")) return true;
            return false;
        }

        public static void Stop()
        {
            FiddlerApplication.BeforeRequest -= SetSessionProxy;
            FiddlerApplication.AfterSessionComplete -= AfterSessionComplete;
            FiddlerApplication.Shutdown();
        }

        private static void Distribute(object o)
        {
            Session oSession = o as Session;
            //System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
            foreach(string key in Handlers.Keys)
            {
                if (oSession.PathAndQuery.Contains(key))
                {
                    //Handlers[key].BeginInvoke(oSession, null, null);
                    foreach(var Handler in Handlers[key].GetInvocationList())
                    {
                        ExceptionCatcherDelegate.BeginInvoke(Handler as Action<Session>, oSession, null, null);
                    }
                    //ExceptionCatcherDelegate.BeginInvoke(Handlers[key], oSession, null, null);
                }
            }
        }

        private static readonly Action<Action<Session>, Session> ExceptionCatcherDelegate = ExceptionCatcher;
        private static void ExceptionCatcher(Action<Session> action, Session parameter)
        {
#if DEBUG == false
            try
            {
#endif
                action(parameter);
#if DEBUG == false
            }
            catch(Exception ex)
            {
                Current.Dispatcher.Invoke(() => System.Windows.MessageBox.Show(ex.Message));
            }
#endif
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
        public System.Timers.Timer Ticker { get; } = new System.Timers.Timer(1000) { Enabled = true };
    }
}
