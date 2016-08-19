using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Huoyaoyuan.AdmiralRoom.Win32;
using mshtml;
using SHDocVw;
using IServiceProvider = Huoyaoyuan.AdmiralRoom.Win32.IServiceProvider;
using Size = System.Windows.Size;
using WebBrowser = System.Windows.Controls.WebBrowser;

namespace Huoyaoyuan.AdmiralRoom
{
    /// <summary>
    /// KanColleBrowser.xaml 的交互逻辑
    /// </summary>
    [ContentProperty(nameof(WebBrowser))]
    public partial class KanColleBrowser : UserControl
    {
        #region StyleSheet
        private const string OverrideStyleSheet =
@"body {
    margin:0;
    overflow:hidden
}

#game_frame {
    position:fixed;
    left:50%;
    top:-16px;
    margin-left:-450px;
    z-index:1
}

.area-pickupgame,
.area-menu
{
    display:none!important;
}";
        #endregion

        private static readonly Size kanColleSize = new Size(800.0, 480.0);
        private static readonly Size browserSize = new Size(960.0, 572.0);
        private static readonly int OriginDpi = 96;
        private double zoomFactor = 1;
        private bool firstLoad = true;
        private IHTMLStyleSheet styleSheet;

        public WebBrowser Browser => this.WebBrowser;

        public bool IsFlashLocked
        {
            get { return (bool)GetValue(IsFlashLockedProperty); }
            set { SetValue(IsFlashLockedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFlashLocked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFlashLockedProperty =
            DependencyProperty.Register(nameof(IsFlashLocked), typeof(bool), typeof(KanColleBrowser),
                new PropertyMetadata(true, (d, e) => ((KanColleBrowser)d).LockFlash((bool)e.NewValue)));

        private void LockFlash(bool islocked)
        {
            ApplyStyleSheet(islocked);
            UpdateSize(islocked);
        }

        public ICommand GotoUrlCommand { get; }

        public KanColleBrowser()
        {
            InitializeComponent();
            WebBrowser.Navigated += (_, __) => ApplyZoomFactor(zoomFactor);
            WebBrowser.LoadCompleted += (_, __) => LockFlash(IsFlashLocked);
            SetSilence(true);
            SetAllowDrop(false);

            btnBack.Click += (_, __) => WebBrowser.GoBack();
            btnFoward.Click += (_, __) => WebBrowser.GoForward();
            GotoUrlCommand = new DelegateCommand(() =>
            {
                if (!txtAddress.Text.Contains(":"))
                    txtAddress.Text = "http://" + txtAddress.Text;
                try
                {
                    WebBrowser.Navigate(txtAddress.Text);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            });
            btnRefresh.Click += (_, __) => WebBrowser.Navigate(WebBrowser.Source);
            btnBackToGame.Click += (_, __) => WebBrowser.Navigate(Properties.Settings.Default.GameUrl);
            btnStop.Click += (_, __) => Stop();
            WebBrowser.Navigating += (_, e) =>
            {
                txtAddress.Text = e.Uri.ToString();
                btnBack.IsEnabled = WebBrowser.CanGoBack;
                btnFoward.IsEnabled = WebBrowser.CanGoForward;
                styleSheet = null;
                UpdateSize(false);
            };
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
                    WebBrowser.Navigate(Properties.Settings.Default.GameUrl);
                    firstLoad = false;
                }
            };
        }
        private void UpdateSize(bool shrink)
        {
            Size dpi = GetSystemDpi();
            if (styleSheet != null && shrink)
            {
                WebBrowser.Width = kanColleSize.Width * zoomFactor * OriginDpi / dpi.Width;
                WebBrowser.Height = kanColleSize.Height * zoomFactor * OriginDpi / dpi.Height;
            }
            else
            {
                WebBrowser.Width = double.NaN;
                WebBrowser.Height = double.NaN;
            }
        }
        public HTMLEmbed FindFlashElement()
        {
            var document = WebBrowser.Document as HTMLDocument;
            if (document == null) return null;

            return FindFlashElementRecursive(document);
        }
        private HTMLEmbed FindFlashElementRecursive(HTMLDocument document)
        {
            var embed = document.getElementsByTagName("embed").OfType<HTMLEmbed>().FirstOrDefault(x => x.src.Contains(".swf?"));
            if (embed != null) return embed;

            var frames = document.frames;
            if (frames == null) return null;
            int count = frames.length;
            for (int i = 0; i < count; i++)
            {
                var item = frames.item(i);
                var provider = item as IServiceProvider;
                if (provider == null) continue;

                object ppvObject;
                provider.QueryService(typeof(IWebBrowserApp).GUID, typeof(IWebBrowser2).GUID, out ppvObject);
                var webBrowser = ppvObject as IWebBrowser2;
                if (webBrowser == null) continue;

                var iframeDocument = webBrowser.Document as HTMLDocument;
                if (iframeDocument == null) continue;

                embed = FindFlashElementRecursive(iframeDocument);
                if (embed != null) return embed;
            }
            return null;
        }
        private void ApplyStyleSheet(bool apply)
        {
            try
            {
                if (apply)
                {
                    var document = WebBrowser.Document as HTMLDocument;
                    if (document == null) return;

                    var gameFrame = document.getElementById("game_frame");
                    if (gameFrame == null) gameFrame = document.getElementById("ooi-game");
                    if (gameFrame == null) gameFrame = document.getElementById("flashWrap");
                    if (gameFrame == null && document.url.Contains(".swf?"))
                        gameFrame = document.body;
                    HTMLDocument target;
                    HTMLEmbed flash;
                    if (gameFrame != null) target = gameFrame.document as HTMLDocument;
                    else if ((flash = FindFlashElement()) != null) target = flash.document;
                    else return;
                    if (FindFlashElement() != null)
                    {
                        styleSheet = target.createStyleSheet();
                        styleSheet.cssText = OverrideStyleSheet;
                    }
                }
                else
                {
                    if (styleSheet != null)
                        styleSheet.cssText = string.Empty;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
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
        public bool TakeScreenShot(string path)
        {
            var document = WebBrowser.Document as HTMLDocument;
            if (document == null) return false;

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                var flash = FindFlashElement();
                var viewObject = flash as IViewObject;
                if (viewObject == null) return false;

                int width = Convert.ToInt32(flash.width);
                int height = Convert.ToInt32(flash.height);
                SaveScreenshot(width, height, viewObject, path);
            }
            catch (Exception ex)
            {
                Models.Status.Current.StatusText = StringTable.Screenshot_Fail + ex;
                return false;
            }
            Models.Status.Current.StatusText = StringTable.Screenshot_Success + path;
            return true;
        }
        private static void SaveScreenshot(int width, int height, IViewObject viewObject, string path)
        {
            using (var image = new Bitmap(width, height, PixelFormat.Format24bppRgb))
            {
                var rect = new RECT { left = 0, top = 0, width = width, height = height };
                var tdevice = new DVTARGETDEVICE { tdSize = 0 };

                using (var graphics = Graphics.FromImage(image))
                {
                    var hdc = graphics.GetHdc();
                    viewObject.Draw(1, 0, IntPtr.Zero, tdevice, IntPtr.Zero, hdc, rect, null, IntPtr.Zero, IntPtr.Zero);
                    graphics.ReleaseHdc(hdc);
                }
                var format = Path.GetExtension(path)?.ToLower() == ".jpg"
                    ? ImageFormat.Jpeg
                    : ImageFormat.Png;

                image.Save(path, format);
            }
        }
        public void ApplyZoomFactor(double zoomFactor)
        {
            this.zoomFactor = zoomFactor;
            try
            {
                var provider = this.WebBrowser.Document as IServiceProvider;
                if (provider == null) return;

                object ppvObject;
                provider.QueryService(typeof(IWebBrowserApp).GUID, typeof(IWebBrowser2).GUID, out ppvObject);
                var webBrowser = ppvObject as IWebBrowser2;
                if (webBrowser == null) return;

                object pvaIn = (int)(zoomFactor * GetSystemDpiRate() * 100);
                webBrowser.ExecWB(OLECMDID.OLECMDID_OPTICAL_ZOOM, OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref pvaIn);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            UpdateSize(true);
        }
        private void SetAllowDrop(bool allowdrop)
        {
            var axIWebBrowser2Prop = typeof(WebBrowser).GetProperty("AxIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            dynamic axIWebBrowser2 = axIWebBrowser2Prop.GetValue(this.WebBrowser);
            axIWebBrowser2.RegisterAsDropTarget = allowdrop;
            //axIWebBrowser2.GetType().InvokeMember("RegisterAsDropTarget", BindingFlags.SetProperty, null, axIWebBrowser2, new object[] { false });
        }
        private void SetSilence(bool issilence)
        {
            var axIWebBrowser2Prop = typeof(WebBrowser).GetProperty("AxIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            dynamic axIWebBrowser2 = axIWebBrowser2Prop.GetValue(this.WebBrowser);
            //axIWebBrowser2.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, axIWebBrowser2, new object[] { issilence });
            axIWebBrowser2.Silent = issilence;
        }
        public void Stop()
        {
            var provider = this.WebBrowser.Document as IServiceProvider;
            if (provider == null) return;

            object ppvObject;
            provider.QueryService(typeof(IWebBrowserApp).GUID, typeof(IWebBrowser2).GUID, out ppvObject);
            var webBrowser = ppvObject as IWebBrowser2;
            if (webBrowser == null) return;

            webBrowser.Stop();
        }
    }
}
