using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Markup;
using mshtml;

namespace Huoyaoyuan.AdmiralRoom
{
    /// <summary>
    /// KanColleBrowser.xaml 的交互逻辑
    /// </summary>
    [ContentProperty("WebBrowser")]
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
}";
        #endregion

        private static readonly Size kanColleSize = new Size(800.0, 480.0);
        private static readonly Size browserSize = new Size(960.0, 572.0);
        private static readonly int OriginDpi = 96;
        private bool styleSheetApplied;

        public WebBrowser Browser => this.WebBrowser;

        public KanColleBrowser()
        {
            InitializeComponent();
            if(!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                this.Loaded += (_, __) => WebBrowser.Navigate(Properties.Settings.Default.GameUrl);
            WebBrowser.LoadCompleted += HandleLoadCompleted;
        }
        private void HandleLoadCompleted(object sender, NavigationEventArgs e)
        {
            this.ApplyStyleSheet();
            //WebBrowserHelper.SetScriptErrorsSuppressed(this.WebBrowser, true);

            this.Update();

            var window = Window.GetWindow(this.WebBrowser);
            if (window != null)
            {
                window.Width = this.WebBrowser.Width;
            }
        }
        private void Update()
        {
            Size dpi = GetSystemDpi();
            if (styleSheetApplied)
            {
                this.WebBrowser.MinWidth = kanColleSize.Width * OriginDpi / dpi.Width;
                this.WebBrowser.MinHeight = kanColleSize.Height * OriginDpi / dpi.Height;
                this.WebBrowser.Width = this.WebBrowser.MinWidth;
                this.WebBrowser.Height = this.WebBrowser.MinHeight;
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
            using (var graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            {
                dpi = new Size(graphics.DpiX, graphics.DpiY);
            }
            return dpi;
        }
    }
}
