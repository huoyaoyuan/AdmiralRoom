using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Modules.Akashi
{
    [Export(typeof(IModule))]
    public class Akashi : IModule
    {
        private class ChildWindow : IChildWindow
        {

            public string Title => "改修工廠";

            public Type WindowType => typeof(AkashiWindow);
        }
        public IEnumerable<IChildView> ChildViews => null;

        public IEnumerable<IChildWindow> ChildWindows => new[] { new ChildWindow() };

        public string Name => "改修工廠";

        public FrameworkElement SettingView => null;

        public void OnCultureChanged(CultureInfo culture)
        {
            //nothing happens
        }
    }
}
