using System;
using System.IO;
using System.Windows;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Environment.CurrentDirectory = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Win32Helper.SetIEEmulation(11001);
            Win32Helper.SetGPURendering(true);
            Win32Helper.SetMMCSSTask();
            Config.Current.MemberwiseCopy(Config.Load());

            Officer.Staff.Start(AdmiralRoom.Properties.Settings.Default.ListenPort);
            Officer.Staff.Proxy = Config.Current.Proxy;

            Officer.Staff.Current.Quests.Load();
            Officer.Staff.Current.MasterData.Load();
            Notifier.SetNotifier(Config.Current.PreferToastNotify);

            Logger.Loggers.Initialize();
            ModuleHost.Instance.Initialize();

            this.MainWindow = new MainWindow();
            DispatcherHelper.UIDispatcher = MainWindow.Dispatcher;
            this.MainWindow.Show();
            Models.Status.Current.StatusText = AdmiralRoom.Properties.Resources.Status_Ready;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
#pragma warning disable CC0022
            using (var file = File.Open("crash.log", FileMode.Append, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(file);
                sw.WriteLine("==================================================");
                sw.WriteLine(e.ExceptionObject.ToString());
                sw.Flush();
#pragma warning restore CC0022
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Officer.Staff.Stop();
            Config.Current.Save();
            Officer.Staff.Current.Quests.Save();
            (Notifier.Current as IDisposable)?.Dispose();
        }
    }
}
