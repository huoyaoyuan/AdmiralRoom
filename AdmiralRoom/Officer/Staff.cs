using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Fiddler;

#pragma warning disable CC0022

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Staff
    {
        private Staff() { }
        private static Staff _current;
        public static Staff Current
        {
            get
            {
                if (_current == null) _current = new Staff();
                return _current;
            }
        }
        public static Proxy Proxy { get; set; }
        private static readonly Dictionary<string, APIObservable> apisource = new Dictionary<string, APIObservable>();
        public static APIObservable API(string apiname)
        {
            APIObservable v;
            apisource.TryGetValue(apiname, out v);
            if (v == null)
            {
                v = new APIObservable();
                apisource.Add(apiname, v);
            }
            return v;
        }
        public static bool IsStarted { get; private set; }
        public static bool Start(int port = 39175)
        {
            if (FiddlerApplication.IsStarted()) FiddlerApplication.Shutdown();
            FiddlerApplication.Startup(port, FiddlerCoreStartupFlags.ChainToUpstreamGateway);
            if (FiddlerApplication.oProxy.ListenPort == 0) return IsStarted = false;

            FiddlerApplication.BeforeRequest += SetSessionProxy;
            FiddlerApplication.AfterSessionComplete += AfterSessionComplete;

            Win32Helper.RefreshIESettings($"localhost:{port}");
            return IsStarted = true;
        }

        private static void AfterSessionComplete(Session oSession)
        {
            if (oSession.PathAndQuery.StartsWith("/kcsapi") && oSession.oResponse.MIMEType.Equals("text/plain"))
            {
                Models.Status.Current.StatusText = string.Format(StringTable.Status_GetResponse, oSession.url);
                DistributeAsync(oSession);
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

        private static readonly object lockObj = new object();
        private static Task DistributeAsync(object o) => Task.Factory.StartNew(() =>
            {
                Session oSession = o as Session;
                lock (lockObj)
                {
                    foreach (string key in apisource.Keys.ToArray())
                        if (oSession.PathAndQuery.EndsWith(key))
                            apisource[key].Handler.GetInvocationList().ForEach(x => ExceptionCatcher(x as Action<Session>, oSession));
                }
            });

        private static void ExceptionCatcher(Action<Session> action, Session parameter)
        {
            try
            {
                action(parameter);
            }
            catch (Exception ex)
            {
                DispatcherHelper.UIDispatcher.InvokeAsync(() => Models.Status.Current.LatestException = ex);
            }
        }
        public class APIObservable
        {
            public Action<Session> Handler;
            public void Subscribe(Action<Session> handler) => Handler += handler;
            public void Subscribe<T>(Action<T> handler) => Subscribe(x =>
            {
                API.APIData<T> svdata;
                if (x.TryParse(out svdata)) handler(svdata.Data);
            });
            public void Subscribe(Action<NameValueCollection> handler) => Subscribe(x =>
            {
                API.APIData svdata;
                if (x.TryParse(out svdata)) handler(svdata.Request);
            });
            public void Subscribe<T>(Action<NameValueCollection, T> handler) => Subscribe(x =>
            {
                API.APIData<T> svdata;
                if (x.TryParse(out svdata)) handler(svdata.Request, svdata.Data);
            });
            //public void SubscribeDynamic(Action<dynamic> handler) => Subscribe(x =>
            //{
            //    API.APIData<dynamic> svdata;
            //    if (x.TryParseDynamic(out svdata)) handler(svdata.Data);
            //});
            //public void SubscribeDynamic(Action<NameValueCollection, dynamic> handler) => Subscribe(x =>
            //{
            //    API.APIData<dynamic> svdata;
            //    if (x.TryParseDynamic(out svdata)) handler(svdata.Request, svdata.Data);
            //});
            public SubObservable<T> Where<T>(Func<T, bool> selector) => new SubObservable<T> { Parent = this, Selector = selector };
        }
        public class SubObservable<T>
        {
            public APIObservable Parent { get; set; }
            public Func<T, bool> Selector { get; set; }
            public void Subscribe(Action<T> handler) => Parent.Subscribe<T>(x => { if (Selector(x)) handler(x); });
        }
        public Admiral Admiral { get; } = new Admiral();
        public Homeport Homeport { get; } = new Homeport();
        public MasterData MasterData { get; } = new MasterData();
        public System.Timers.Timer Ticker { get; } = new System.Timers.Timer(1000) { Enabled = true };
        public Shipyard Shipyard { get; } = new Shipyard();
        public QuestManager Quests { get; } = new QuestManager();
        public BattleManager Battle { get; } = new BattleManager();
    }
}
