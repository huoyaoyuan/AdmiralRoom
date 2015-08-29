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

            //Browser button handler
            GameHost.WebBrowser.Navigating += (_, e) =>
            {
                BrowserAddr.Text = e.Uri.AbsoluteUri;
                BrowserBack.IsEnabled = GameHost.WebBrowser.CanGoBack;
                BrowserForward.IsEnabled = GameHost.WebBrowser.CanGoForward;
            };
            BrowserBack.Click += (_, __) => GameHost.WebBrowser.GoBack();
            BrowserForward.Click += (_, __) => GameHost.WebBrowser.GoForward();
            BrowserGoto.Click += (_, __) =>
            {
                if (!BrowserAddr.Text.Contains(":"))
                    BrowserAddr.Text = "http://" + BrowserAddr.Text;
                try
                {
                    GameHost.WebBrowser.Navigate(BrowserAddr.Text);
                }
                catch { }
            };
            BrowserRefresh.Click += (_, __) => GameHost.WebBrowser.Refresh();
            BrowserBackToGame.Click += (_, __) => GameHost.WebBrowser.Navigate(Properties.Settings.Default.GameUrl);

            //Theme button handler
            NoDWM.Click += (s, _) =>this.DontUseDwm = (s as CheckBox).IsChecked.Value;
            Themes.ItemsSource = ThemeService.SupportedThemes;
            Themes.SelectionChanged += (s, _) =>ThemeService.ChangeTheme((s as ComboBox).SelectedValue.ToString());
            Themes.SelectedIndex = 0;
            UseAeroControl.Click += (s, _) =>ThemeService.EnableAeroControls((s as CheckBox).IsChecked.Value);
            
        }
    }
}
