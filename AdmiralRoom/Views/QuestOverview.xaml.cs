using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Views
{
    /// <summary>
    /// QuestOverview.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(ISubView))]
    public partial class QuestOverview : UserControl, ISubView
    {
        public QuestOverview()
        {
            InitializeComponent();
        }

        public string ContentID => nameof(QuestOverview);

        public UIElement View => this;

        public string GetTitle(CultureInfo culture) => Properties.Resources.ViewTitle_QuestOverview;
    }
}
