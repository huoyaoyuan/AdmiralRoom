using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
            Helper.SetIEEmulation(11001);
            Helper.SetGPURendering(true);
            Helper.SetMMCSSTask();
            Config.Current = Config.Load();

            Officer.Staff.Current.Start(AdmiralRoom.Properties.Settings.Default.ListenPort);
            Officer.Staff.Current.Proxy = Config.Current.Proxy;

            this.MainWindow = new MainWindow();
            this.MainWindow.Show();
            Models.StatusModel.Current.StatusText = "就绪";
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Officer.Staff.Current.Stop();
            Config.Current.Save();
        }
    }
}
