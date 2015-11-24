using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

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
            BrowserRefresh.Click += (_, __) => GameHost.WebBrowser.Navigate(GameHost.WebBrowser.Source);
            BrowserBackToGame.Click += (_, __) => GameHost.WebBrowser.Navigate(Properties.Settings.Default.GameUrl);

            //Language handler
            LanguageBox.ItemsSource = ResourceService.SupportedCultures;
            LanguageBox.SelectionChanged += (s, _) => ResourceService.Current.ChangeCulture((s as ComboBox).SelectedValue.ToString());
            ResourceService.Current.ChangeCulture(LanguageBox.SelectedValue.ToString());

            //Theme button handler
            NoDWM.Click += (s, _) => this.DontUseDwm = (s as CheckBox).IsChecked.Value;
            NoDWM.IsChecked = this.DontUseDwm = Config.Current.NoDWM;
            Themes.ItemsSource = ThemeService.SupportedThemes;
            Themes.SelectionChanged += (s, _) => ThemeService.ChangeRibbonTheme((s as ComboBox).SelectedValue.ToString());
            ThemeService.ChangeRibbonTheme(Themes.SelectedValue.ToString());
            UseAeroControl.Click += (s, _) => ThemeService.EnableAeroControls((s as CheckBox).IsChecked.Value);
            UseAeroControl.IsChecked = Config.Current.Aero;
            ThemeService.EnableAeroControls(Config.Current.Aero);

            //Proxy button handler
            UpdateProxySetting.Click += (_, __) =>
            {
                Config.Current.Proxy.Host = ProxyHost.Text;
                Config.Current.Proxy.Port = int.Parse(ProxyPort.Text);
                Config.Current.EnableProxy = EnableProxy.IsChecked.Value;
                Config.Current.HTTPSProxy.Host = ProxyHostHTTPS.Text;
                Config.Current.HTTPSProxy.Port = int.Parse(ProxyPortHTTPS.Text);
                Config.Current.EnableProxyHTTPS = EnableProxyHTTPS.IsChecked.Value;
            };
            CancelProxySetting.Click += (_, __) =>
            {
                ProxyHost.Text = Config.Current.Proxy.Host;
                ProxyPort.Text = Config.Current.Proxy.Port.ToString();
                EnableProxy.IsChecked = Config.Current.EnableProxy;
                ProxyHostHTTPS.Text = Config.Current.HTTPSProxy.Host;
                ProxyPortHTTPS.Text = Config.Current.HTTPSProxy.Port.ToString();
                EnableProxyHTTPS.IsChecked = Config.Current.EnableProxyHTTPS;
            };

            //Font handler
            var FontFamilies = (new System.Drawing.Text.InstalledFontCollection()).Families;
            List<string> FontNames = new List<string>();
            foreach (var font in FontFamilies)
            {
                FontNames.Add(font.Name);
            }
            SelectFontFamily.ItemsSource = FontNames;
            SelectFontFamily.SelectionChanged += (s, _) =>
            {
                try
                {
                    this.FontFamily = new FontFamily((s as ComboBox).SelectedValue.ToString());
                }
                catch { }
            };
            SelectFontFamily.SelectedValue = "等线";
            TextFontSize.DataContext = this;
            TextFontSize.SetBinding(TextBox.TextProperty, new Binding { Source = this.ribbonWindow, Path = new PropertyPath("FontSize"), Mode = BindingMode.TwoWay });
            FontLarge.Click += (_, __) => this.FontSize += 1;
            FontSmall.Click += (_, __) => this.FontSize -= 1;

            ScreenShotButton.Click += (_, __) => GameHost.TakeScreenShot(Config.Current.GenerateScreenShotFileName());

            this.Loaded += (_, __) => GameHost.Browser.Navigate(Properties.Settings.Default.GameUrl);
            this.Loaded += (_, __) => Win32Helper.GetRestoreWindowPosition(this);
            this.Closing += (_, __) => Win32Helper.SetRestoreWindowPosition(this);
        }

        private void MakeViewList(ILayoutElement elem)
        {
            if (elem is LayoutAnchorable)
            {
                ViewList.Add((elem as LayoutAnchorable).ContentId, elem as LayoutAnchorable);
                return;
            }
            if (elem is ILayoutContainer)
            {
                foreach (var child in (elem as ILayoutContainer).Children)
                {
                    MakeViewList(child);
                }
            }
        }

        private void LoadLayout(object sender, RoutedEventArgs e)
        {
            var s = new XmlLayoutSerializer(DockMan);
            s.LayoutSerializationCallback += (_, args) => args.Content = args.Content;
            try
            {
                s.Deserialize("layout.xml");
            }
            catch { }
            foreach (var view in DockMan.Layout.Hidden.Where(x => x.PreviousContainerIndex == -1).ToArray())
            {
                DockMan.Layout.Hidden.Remove(view);
            }
            MakeViewList(DockMan.Layout);
        }
        private void SaveLayout(object sender, EventArgs e)
        {
            var s = new XmlLayoutSerializer(DockMan);
            s.Serialize("layout.xml");
        }

        private Dictionary<string, LayoutAnchorable> ViewList = new Dictionary<string, LayoutAnchorable>();
        private void SetToggleBinding(object sender, RoutedEventArgs e)
        {
            Binding ToggleBinding = new Binding();
            Control content = (sender as Control).Tag as Control;
            string ViewName = content.GetType().Name;
            LayoutAnchorable TargetView;
            if (!ViewList.TryGetValue(ViewName, out TargetView))
            {
                TargetView = new LayoutAnchorable();
                ViewList.Add(ViewName, TargetView);
                TargetView.AddToLayout(DockMan, AnchorableShowStrategy.Most);
                TargetView.Float();
                TargetView.Hide();
            }
            if (TargetView.Content == null)
            {
                TargetView.Content = content;
                if (content.DataContext == null)
                    content.DataContext = Officer.Staff.Current;
                TargetView.ContentId = ViewName;
                TargetView.Title = ViewName;
                TargetView.FloatingHeight = content.Height;
                TargetView.FloatingWidth = content.Width;
                TargetView.FloatingTop = this.ActualHeight / 2;
                TargetView.FloatingWidth = this.ActualWidth / 2;
                Binding titlebinding = new Binding("Resources.ViewTitle_" + ViewName);
                titlebinding.Source = ResourceService.Current;
                BindingOperations.SetBinding(TargetView, LayoutAnchorable.TitleProperty, titlebinding);
                (sender as Fluent.ToggleButton).SetBinding(Fluent.ToggleButton.HeaderProperty, titlebinding);
            }
            ToggleBinding.Source = TargetView;
            ToggleBinding.Path = new PropertyPath("IsVisible");
            ToggleBinding.Mode = BindingMode.TwoWay;
            (sender as Fluent.ToggleButton).SetBinding(Fluent.ToggleButton.IsCheckedProperty, ToggleBinding);
        }

        private void SetUniqueWindowCommand(object sender, RoutedEventArgs e)
        {
            Button control = sender as Button;
            Type windowtype = control.Tag as Type;
            if (windowtype == null) return;

            control.Content = windowtype.Name;
            Binding titlebinding = new Binding("Resources.ViewTitle_" + windowtype.Name);
            titlebinding.Source = ResourceService.Current;
            control.SetBinding(ContentProperty, titlebinding);

            control.Click += UniqueCommandClick;
        }

        private void UniqueCommandClick(object sender, RoutedEventArgs e)
        {
            Button control = sender as Button;
            Window w;
            if (control.Tag is Type)
            {
                w = Activator.CreateInstance(control.Tag as Type) as Window;
                w.Closed += (_, __) => control.Tag = w.GetType();
                Binding titlebinding = new Binding("Resources.ViewTitle_" + w.GetType().Name);
                titlebinding.Source = ResourceService.Current;
                w.SetBinding(TitleProperty, titlebinding);
                control.Tag = w;
            }
            else w = control.Tag as Window;
            w.Show();
            w.Activate();
        }

        private void SetBrowserZoomFactor(object sender, RoutedPropertyChangedEventArgs<double> e)
            => this.GameHost.ApplyZoomFactor(e.NewValue);
    }
}
