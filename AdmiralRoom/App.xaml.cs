using System;
using System.IO;
using System.Linq;
using System.Windows;
using Huoyaoyuan.AdmiralRoom.Composition;
using Huoyaoyuan.AdmiralRoom.Notifier;

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

            var rootfolder = new DirectoryInfo(".");
            foreach (var file in rootfolder.GetFiles("*.old", SearchOption.AllDirectories))
                if (file.Name.EndsWith(".xml.old"))
                    file.MoveTo(file.FullName.Replace(".old", ""));
                else file.Delete();

            Win32Helper.SetIEEmulation(11001);
            Win32Helper.SetGPURendering(true);
            Win32Helper.SetMMCSSTask();
            Config.Current.MemberwiseCopy(Config.Load());

            var random = new Random();
            for (int listenPort = AdmiralRoom.Properties.Settings.Default.ListenPort; !Officer.Staff.Start(listenPort); listenPort = random.Next() % 32768 + 32768)
                if (MessageBox.Show($"Listen on port {listenPort} failed. Change and retry?", "", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    break;

            Officer.Staff.Proxy = Config.Current.Proxy;
            Reporter.KancolleDBReporter.Initialize();

            Officer.Staff.Current.Quests.Load();
            Officer.Staff.Current.MasterData.Load();

            Logger.Loggers.Initialize();

            this.MainWindow = new NewWindow();
            DispatcherHelper.UIDispatcher = MainWindow.Dispatcher;
            this.MainWindow.Show();
            Models.Status.Current.StatusText = StringTable.Status_Ready;

            Updater.Updater.Instance.Timer.Start();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
#pragma warning disable CC0022
            using (var file = File.Open("crash.log", FileMode.Append, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(file);
                sw.WriteLine("==================================================");
                sw.WriteLine(e.ExceptionObject);
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
            Officer.RankingViewModel.Instance.Save();
            (NotifierFactories.Current as IDisposable)?.Dispose();
            ModuleHost.Instance.Dispose();
        }
    }
}
