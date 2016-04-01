using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;
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
        private bool styleSheetApplied;
        private double zoomFactor = 1;

        public WebBrowser Browser => this.WebBrowser;

        public KanColleBrowser()
        {
            InitializeComponent();
            WebBrowser.Navigated += (_, __) => ApplyZoomFactor(zoomFactor);
            WebBrowser.LoadCompleted += HandleLoadCompleted;
            SetSilence(true);
            SetAllowDrop(false);
        }
        private void HandleLoadCompleted(object sender, NavigationEventArgs e)
        {
            this.ApplyStyleSheet();

            this.UpdateSize();

            var window = Window.GetWindow(this.WebBrowser);
            if (window != null)
            {
                window.Width = this.WebBrowser.Width;
            }
        }
        private void UpdateSize()
        {
            Size dpi = GetSystemDpi();
            if (styleSheetApplied)
            {
                this.WebBrowser.Width = kanColleSize.Width * zoomFactor * OriginDpi / dpi.Width;
                this.WebBrowser.Height = kanColleSize.Height * zoomFactor * OriginDpi / dpi.Height;
                //this.WebBrowser.Width = this.WebBrowser.MinWidth;
                //this.WebBrowser.Height = this.WebBrowser.MinHeight;
            }
        }
        private void ApplyStyleSheet()
        {
            try
            {
                var document = this.WebBrowser.Document as HTMLDocument;
                if (document == null) return;

                var gameFrame = document.getElementById("game_frame");
                if (gameFrame == null)
                {
                    if (document.url.Contains(".swf?"))
                    {
                        gameFrame = document.body;
                    }
                }

                if (gameFrame != null)
                {
                    var target = gameFrame.document as HTMLDocument;
                    if (target != null)
                    {
                        target.createStyleSheet().cssText = OverrideStyleSheet;
                        this.styleSheetApplied = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return;
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

        /// <remarks>
        /// 本スクリーンショット機能は、「艦これ 司令部室」開発者の @haxe さんより多大なご協力を頂き実装できました。
        /// ありがとうございました。
        /// </remarks>
        public bool TakeScreenShot(string path)
        {
            var document = this.WebBrowser.Document as HTMLDocument;
            if (document == null)
            {
                return false;
            }

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                if (document.url.Contains(".swf?"))
                {
                    var viewObject = document.getElementsByTagName("embed").item(0, 0) as IViewObject;
                    if (viewObject == null)
                        return false;

                    var width = ((HTMLEmbed)viewObject).clientWidth;
                    var height = ((HTMLEmbed)viewObject).clientHeight;
                    SaveScreenshot(width, height, viewObject, path);
                }
                else
                {
                    var gameFrame = document.getElementById("game_frame").document as HTMLDocument;
                    if (gameFrame == null)
                        return false;

                    var frames = document.frames;
                    var find = false;
                    for (var i = 0; i < frames.length; i++)
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

                        //flash要素が<embed>である場合と<object>である場合を判別して抽出
                        IViewObject viewObject = null;
                        int width = 0, height = 0;
                        var swf = iframeDocument.getElementById("externalswf");
                        if (swf == null) continue;
                        Func<dynamic, bool> function = target =>
                        {
                            if (target == null) return false;
                            viewObject = target as IViewObject;
                            if (viewObject == null) return false;
                            width = int.Parse(target.width);
                            height = int.Parse(target.height);
                            return true;
                        };
                        if (!function(swf as HTMLEmbed) && !function(swf as HTMLObjectElement)) continue;

                        find = true;
                        SaveScreenshot(width, height, viewObject, path);

                        break;
                    }

                    if (!find)
                        return false;
                }
            }
            catch (Exception ex)
            {
                Models.Status.Current.StatusText = Properties.Resources.Screenshot_Fail + ex;
                return false;
            }
            Models.Status.Current.StatusText = Properties.Resources.Screenshot_Success + path;
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
            UpdateSize();
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
    }
}
