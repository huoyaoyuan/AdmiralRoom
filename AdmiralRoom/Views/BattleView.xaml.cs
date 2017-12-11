using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Huoyaoyuan.AdmiralRoom.Composition;
using Huoyaoyuan.AdmiralRoom.Logger;
using Huoyaoyuan.AdmiralRoom.Officer;
using Huoyaoyuan.AdmiralRoom.Views.Standalone;

namespace Huoyaoyuan.AdmiralRoom.Views
{
    /// <summary>
    /// BattleView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(ISubView))]
    public partial class BattleView : UserControl, ISubView
    {
        public BattleView()
        {
            InitializeComponent();
        }

        public string ContentID => nameof(BattleView);

        public UIElement View => this;

        public string GetTitle(CultureInfo culture) => StringTable.ViewTitle_BattleView;

        private void ShowBattleDetail(object sender, RoutedEventArgs e)
        {
            var battleMan = (DataContext as Staff)?.Battle;
            var vm = new BattleDetailViewModel
            {
                Battle = battleMan.CurrentBattle,
                Node = battleMan.CurrentNode,
                Time = DateTimeOffset.Now
            };
            new BattleDetailView { DataContext = vm }.Show();
        }
    }
}
