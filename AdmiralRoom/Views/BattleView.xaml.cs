using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Huoyaoyuan.AdmiralRoom.Composition;

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
    }
}
