using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Size = System.Windows.Size;

namespace Huoyaoyuan.AdmiralRoom
{
    /// <summary>
    /// KanColleBrowser.xaml 的交互逻辑
    /// </summary>
    public partial class KanColleBrowser : UserControl
    {
        private static readonly int OriginDpi = 96;
        private double zoomFactor = 1;
        private bool firstLoad = true;

        public bool IsFlashLocked
        {
            get { return (bool)GetValue(IsFlashLockedProperty); }
            set { SetValue(IsFlashLockedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFlashLocked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFlashLockedProperty =
            DependencyProperty.Register(nameof(IsFlashLocked), typeof(bool), typeof(KanColleBrowser),
                new PropertyMetadata(true));

        public ICommand GotoUrlCommand { get; }

        private const string styleJs = @"var sheet = document.createElement('style');
sheet.innerHTML = '#game_frame { position: fixed; left: 0; top: -16px; z-index: 255; }';
document.body.appendChild(sheet);";

        private readonly CefSharp.WinForms.ChromiumWebBrowser Browser;
        private bool gameLocked;
        public KanColleBrowser()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;

            InitializeComponent();
            Browser = new CefSharp.WinForms.ChromiumWebBrowser("");
            WinFormHost.Child = Browser;
            ApplyZoomFactor(zoomFactor);

            Browser.AddressChanged += (s, e) => Dispatcher.Invoke(() => txtAddress.Text = e.Address);
            Browser.IsBrowserInitializedChanged += (s, e) => ApplyZoomFactor(zoomFactor);
            Browser.LoadingStateChanged += (s, e) =>
            {
                ApplyZoomFactor(zoomFactor);
                if (e.Browser.GetFrame("game_frame") != null)
                {
                    e.Browser.MainFrame.ExecuteJavaScriptAsync(styleJs);
                    gameLocked = true;
                }
                else
                    gameLocked = false;
                //UpdateBrowserSize();
            };

            //btnBack.Click += (_, __) => WebBrowser.GoBack();
            //btnFoward.Click += (_, __) => WebBrowser.GoForward();
            GotoUrlCommand = new DelegateCommand(() =>
            {
                if (!txtAddress.Text.Contains(":"))
                    txtAddress.Text = "http://" + txtAddress.Text;
                try
                {
                    //Browser.Address = txtAddress.Text;
                    Browser.Load(txtAddress.Text);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            });
            btnRefresh.Click += (_, __) => Browser.GetBrowser().Reload();
            btnBackToGame.Click += (_, __) => Browser.Load(Properties.Settings.Default.GameUrl);
            //Browser.Navigating += (_, e) =>
            //{
            //    txtAddress.Text = e.Uri.AbsoluteUri;
            //};
            btnScreenShot.Click += (_, __) => TakeScreenShot(Config.Current.GenerateScreenShotFileName());
            btnCleanCache.Click += async (sender, _) =>
            {
                var button = sender as Button;
                button.IsEnabled = false;
                if (MessageBox.Show(StringTable.CleanCache_Alert, "", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
                    if (await WinInetHelper.DeleteInternetCacheAsync())
                        MessageBox.Show(StringTable.CleanCache_Success);
                    else MessageBox.Show(StringTable.CleanCache_Fail);
                button.IsEnabled = true;
            };

            this.Loaded += (_, __) =>
            {
                if (firstLoad && Officer.Staff.IsStarted)
                {
                    var url = Config.Current.OverrideGameUrl;
                    if (string.IsNullOrWhiteSpace(url)) url = Properties.Settings.Default.GameUrl;
                    txtAddress.Text = url;
                    Browser.Load(url);
                    firstLoad = false;
                }
            };
        }
        private static Size GetSystemDpi()
        {
            Size dpi;
            using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                dpi = new Size(graphics.DpiX, graphics.DpiY);
            }
            return dpi;
        }
        private static double GetSystemDpiRate()
        {
            var dpi = GetSystemDpi();
            return dpi.Height == dpi.Width ? dpi.Height / OriginDpi : 1;
        }
        private void UpdateBrowserSize()
        {
            if (gameLocked)
            {
                double rate = GetSystemDpiRate();
                WinFormHost.Width = 1200 * zoomFactor / rate;
                WinFormHost.Height = 700 * zoomFactor / rate;
            }
            else
            {
                WinFormHost.Width = double.NaN;
                WinFormHost.Height = double.NaN;
            }
        }
        public bool TakeScreenShot(string path)
        {
            //var document = WebBrowser.Document as HTMLDocument;
            //if (document == null) return false;

            //try
            //{
            //    Directory.CreateDirectory(Path.GetDirectoryName(path));
            //    var flash = FindFlashElement();
            //    var viewObject = flash as IViewObject;
            //    if (viewObject == null) return false;

            //    int width = Convert.ToInt32(flash.width);
            //    int height = Convert.ToInt32(flash.height);
            //    SaveScreenshot(width, height, viewObject, path);
            //}
            //catch (Exception ex)
            //{
            //    Models.Status.Current.StatusText = StringTable.Screenshot_Fail + ex;
            //    return false;
            //}
            //Models.Status.Current.StatusText = StringTable.Screenshot_Success + path;
            //return true;
            return false;
        }
        public void ApplyZoomFactor(double zoomFactor)
        {
            this.zoomFactor = zoomFactor;
            if (Browser.IsBrowserInitialized)
                Browser.GetBrowser().GetHost().SetZoomLevel((zoomFactor - GetSystemDpiRate()) / 0.25);
            //UpdateBrowserSize();
        }
    }
}
