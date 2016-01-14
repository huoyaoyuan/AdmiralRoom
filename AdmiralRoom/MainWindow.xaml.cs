using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.WindowsAPICodePack.Dialogs;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

#pragma warning disable CC0021
#pragma warning disable CC0108

namespace Huoyaoyuan.AdmiralRoom
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            ThemeService.ChangeRibbonTheme(Config.Current.Theme, this);

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

            //Theme handler
            Config.Current.NoDwmChanged += v => this.DontUseDwm = v;
            this.DontUseDwm = Config.Current.NoDWM;
            Themes.SelectionChanged += (s, _) =>
            {
                if (DockMan.FloatingWindows.Any())//Can't DestroyWindow
                    MessageBox.Show("有子窗口处于浮动状态，主题切换必须下次启动程序才能生效。", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                {
                    backstage.IsOpen = false;
                    ThemeService.ChangeRibbonTheme(Config.Current.Theme, this);
                }
            };
            Config.Current.AeroChanged += ThemeService.EnableAeroControls;
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
            FontLarge.Click += (_, __) => Config.Current.FontSize++;
            FontSmall.Click += (_, __) => Config.Current.FontSize--;

            ScreenShotButton.Click += (_, __) => GameHost.TakeScreenShot(Config.Current.GenerateScreenShotFileName());
            DeleteCacheButton.Click += async (sender, __) =>
            {
                var button = sender as Button;
                button.IsEnabled = false;
                if (MessageBox.Show(Properties.Resources.CleanCache_Alert, "", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
                    if (await WinInetHelper.DeleteInternetCacheAsync())
                        MessageBox.Show(Properties.Resources.CleanCache_Success);
                    else MessageBox.Show(Properties.Resources.CleanCache_Fail);
                button.IsEnabled = true;
            };

            this.Loaded += (_, __) => GameHost.Browser.Navigate(Properties.Settings.Default.GameUrl);
            this.Loaded += (_, __) => Win32Helper.GetRestoreWindowPosition(this);
            this.Closing += (_, __) => Win32Helper.SetRestoreWindowPosition(this);

            DockCommands = new Config.CommandSet
            {
                Save = new DelegateCommand(() => TrySaveLayout()),
                Load = new DelegateCommand(() => TryLoadLayout()),
                SaveAs = new DelegateCommand(() =>
                {
                    using (var filedialog = new CommonSaveFileDialog())
                    {
                        filedialog.InitialDirectory = Environment.CurrentDirectory;
                        filedialog.DefaultFileName = "config.xml";
                        filedialog.Filters.Add(new CommonFileDialogFilter("Xml Files", "*.xml"));
                        filedialog.Filters.Add(new CommonFileDialogFilter("All Files", "*"));
                        if (filedialog.ShowDialog() == CommonFileDialogResult.Ok)
                            TrySaveLayout(filedialog.FileName);
                    }
                }),
                LoadFrom = new DelegateCommand(() =>
                {
                    using (var filedialog = new CommonOpenFileDialog())
                    {
                        filedialog.InitialDirectory = Environment.CurrentDirectory;
                        filedialog.Filters.Add(new CommonFileDialogFilter("Xml Files", "*.xml"));
                        filedialog.Filters.Add(new CommonFileDialogFilter("All Files", "*"));
                        if (filedialog.ShowDialog() == CommonFileDialogResult.Ok)
                            TryLoadLayout(filedialog.FileName);
                    }
                })
            };
            SetToggleBindings += () =>
                BindingOperations.SetBinding(ViewList["GameHost"], LayoutContent.TitleProperty, new Views.Extensions.LocalizableExtension("Browser"));
        }

        private void MakeViewList(ILayoutElement elem)
        {
            if (elem is LayoutContent)
            {
                ViewList[(elem as LayoutContent).ContentId] = elem as LayoutContent;
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

        #region Layout
        private void LoadLayout(object sender, RoutedEventArgs e)
        {
            TryLoadLayout();
            foreach (var view in DockMan.Layout.Hidden.Where(x => x.PreviousContainerIndex == -1).ToArray())
            {
                DockMan.Layout.Hidden.Remove(view);
            }
        }
        private void SaveLayout(object sender, EventArgs e) => TrySaveLayout();
        private void TryLoadLayout(string path = "layout.xml")
        {
            XmlLayoutSerializer layoutserializer = new XmlLayoutSerializer(DockMan);
            layoutserializer.LayoutSerializationCallback += (_, args) => args.Model.Content = args.Content;
            try
            {
                layoutserializer.Deserialize(path);
            }
            catch { }
            MakeViewList(DockMan.Layout);
            SetToggleBindings();
        }
        private void TrySaveLayout(string path = "layout.xml")
        {
            XmlLayoutSerializer layoutserializer = new XmlLayoutSerializer(DockMan);
            try
            {
                layoutserializer.Serialize(path);
            }
            catch { }
        }
        #endregion

        private readonly Dictionary<string, LayoutContent> ViewList = new Dictionary<string, LayoutContent>();
        private Action SetToggleBindings;
        private void RegisterToggleBinding(object sender, RoutedEventArgs e)
        {
            SetToggleBindings += () => SetToggleBinding(sender as Fluent.ToggleButton);
            SetToggleBinding(sender as Fluent.ToggleButton);
            (sender as Control).Loaded -= RegisterToggleBinding;
        }
        private void SetToggleBinding(Fluent.ToggleButton sender)
        {
            Binding ToggleBinding = new Binding();
            Control content = sender.Tag as Control;
            string ViewName = content.GetType().Name;
            LayoutContent TargetContent;
            LayoutAnchorable TargetView;
            ViewList.TryGetValue(ViewName, out TargetContent);
            TargetView = TargetContent as LayoutAnchorable;
            if (TargetView == null)
            {
                TargetView = new LayoutAnchorable();
                ViewList.Add(ViewName, TargetView);
                TargetView.AddToLayout(DockMan, AnchorableShowStrategy.Most);
                TargetView.DockAsDocument();
                TargetView.CanClose = false;
                TargetView.Hide();
            }
            if (TargetView.Content == null)
            {
                TargetView.Content = content;
                if (content.DataContext == null)
                    content.DataContext = Officer.Staff.Current;
                TargetView.ContentId = ViewName;
                TargetView.Title = ViewName;
                TargetView.CanAutoHide = true;
                TargetView.FloatingHeight = content.Height;
                TargetView.FloatingWidth = content.Width;
                //TargetView.FloatingTop = this.ActualHeight / 2;
                //TargetView.FloatingWidth = this.ActualWidth / 2;
                Binding titlebinding = new Views.Extensions.LocalizableExtension("ViewTitle_" + ViewName);
                BindingOperations.SetBinding(TargetView, LayoutAnchorable.TitleProperty, titlebinding);
                sender.SetBinding(Fluent.ToggleButton.HeaderProperty, titlebinding);
            }
            ToggleBinding.Source = TargetView;
            ToggleBinding.Path = new PropertyPath("IsVisible");
            ToggleBinding.Mode = BindingMode.TwoWay;
            sender.SetBinding(Fluent.ToggleButton.IsCheckedProperty, ToggleBinding);
        }

        private void SetUniqueWindowCommand(object sender, RoutedEventArgs e)
        {
            Button control = sender as Button;
            Type windowtype = control.Tag as Type;
            if (windowtype == null) return;

            control.Content = windowtype.Name;
            Binding titlebinding = new Binding("Resources.ViewTitle_" + windowtype.Name)
            {
                Source = ResourceService.Current
            };
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
                Binding titlebinding = new Binding("Resources.ViewTitle_" + w.GetType().Name)
                {
                    Source = ResourceService.Current
                };
                w.SetBinding(TitleProperty, titlebinding);
                control.Tag = w;
            }
            else w = control.Tag as Window;
            w.Show();
            w.Activate();
        }

        private void SetShowLogger(object sender,RoutedEventArgs e)
        {
            Button control = sender as Fluent.Button;
            dynamic datacontext = control.Tag;
            Binding titlebinding = new Binding("Resources." + datacontext.TitleKey)
            {
                Source = ResourceService.Current
            };
            control.SetBinding(Fluent.Button.HeaderProperty, titlebinding);
        }

        private void ShowLogger(object sender, RoutedEventArgs e)
        {
            Button control = sender as Button;
            Window w;
            if (control.Tag.GetType().IsGenericType)
            {
                var valuetype = control.Tag.GetType().GetGenericArguments()[0];
                dynamic datacontext = typeof(Logger.ViewProvider<>).MakeGenericType(valuetype)
                    .GetConstructor(new[] { control.Tag.GetType() })
                    .Invoke(new[] { control.Tag });
                w = new Views.Standalone.LogView
                {
                    DataContext = datacontext
                };
                (w as Views.Standalone.LogView).SetGridColumns(datacontext.ViewColumns);
                w.Closed += (_, __) => control.Tag = control.Tag;
                w.SetBinding(TitleProperty, new Binding("Header")
                {
                    Source = sender,
                    Mode = BindingMode.OneWay
                });
            }
            else w = control.Tag as Window;
            w.Show();
            w.Activate();
        }

        private void SetBrowserZoomFactor(object sender, RoutedPropertyChangedEventArgs<double> e)
            => this.GameHost.ApplyZoomFactor(e.NewValue);

        public Config.CommandSet DockCommands { get; }
    }
}
