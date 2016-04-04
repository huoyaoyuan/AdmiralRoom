using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using Huoyaoyuan.AdmiralRoom.Composition;

#pragma warning disable CC0021

namespace Huoyaoyuan.AdmiralRoom.Modules.Ranking
{
    [Export(typeof(IModule))]
    public class Ranking : IModule
    {
        public Ranking()
        {
            RankingViewModel.Instance = new RankingViewModel();
        }
        private class ChildView : IChildView
        {
            public string ContentId => "RankingView";

            public string Title => "戦果";

            public FrameworkElement View { get; } = new RankingView();
        }
        public IEnumerable<IChildView> ChildViews => new[] { new ChildView() };

        public IEnumerable<IChildWindow> ChildWindows => null;

        public string Name => "戦果情報";

        public FrameworkElement SettingView => null;

        public void OnCultureChanged(CultureInfo culture)
        {
            //nothing happens
        }

        public void Unload()
        {
            RankingViewModel.Instance.Save();
        }
    }
}
