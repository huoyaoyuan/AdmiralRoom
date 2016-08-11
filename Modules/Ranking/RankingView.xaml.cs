using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Modules.Ranking
{
    /// <summary>
    /// RankingView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(ISubView))]
    public partial class RankingView : UserControl, ISubView
    {
        public RankingView()
        {
            InitializeComponent();
        }

        public string ContentID => "RankingView";

        public UIElement View => this;

        public string GetTitle(CultureInfo culture)
        {
            switch (culture.Name.ToLowerInvariant())
            {
                case "en":
                    return "Ranking";
                case "zh-hans":
                    return "战果";
                default:
                    return "戦果";
            }
        }
    }
}
