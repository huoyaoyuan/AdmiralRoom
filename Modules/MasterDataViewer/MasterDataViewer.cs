using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Modules.MasterDataViewer
{
    [Export(typeof(IModule))]
    public class MasterDataViewer : IModule
    {
        private class ChildWindow : IChildWindow
        {
            public string Title => "MasterData";

            public Type WindowType => typeof(MasterDataWindow);
        }
        public IEnumerable<IChildView> ChildViews => null;

        public IEnumerable<IChildWindow> ChildWindows => new[] { new ChildWindow() };

        public string Name => "MasterDataViewer";

        public FrameworkElement SettingView => null;

        public void OnCultureChanged(CultureInfo culture)
        {
            //nothing happens
        }

        public void Unload()
        {
            //nothing happens
        }
    }
}
