using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Huoyaoyuan.AdmiralRoom
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            GameHost.WebBrowser.Navigating += (_, e) => BrowserAddr.Text = e.Uri.AbsoluteUri;
            BrowserBack.Click += (_, __) => GameHost.WebBrowser.GoBack();
            BrowserForward.Click += (_, __) => GameHost.WebBrowser.GoForward();
            BrowserGoto.Click += (_, __) => GameHost.WebBrowser.Navigate(BrowserAddr.Text);
            BrowserRefresh.Click += (_, __) => GameHost.WebBrowser.Refresh();
            BrowserBackToGame.Click += (_, __) => GameHost.WebBrowser.Navigate(Properties.Settings.Default.GameUrl);
        }
    }
}
