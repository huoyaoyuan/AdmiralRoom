using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Huoyaoyuan.AdmiralRoom
{
    /// <summary>
    /// NewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewWindow : Window
    {
        public NewWindow()
        {
            InitializeComponent();
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
            SetToggleBindings += () => SetToggleBinding(sender as ToggleButton);
            SetToggleBinding(sender as ToggleButton);
            (sender as Control).Loaded -= RegisterToggleBinding;
        }
        private void SetToggleBinding(ToggleButton sender)
        {
            var content = sender.Tag as FrameworkElement;
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
                BindingOperations.SetBinding(TargetView, LayoutContent.TitleProperty, titlebinding);
            }
            sender.SetBinding(ToggleButton.IsCheckedProperty,
                new Binding(nameof(LayoutAnchorable.IsVisible)) { Source = TargetView, Mode = BindingMode.TwoWay });
        }

        private void SetBrowserZoomFactor(object sender, RoutedPropertyChangedEventArgs<double> e)
            => this.GameHost.ApplyZoomFactor(e.NewValue);
    }
}
