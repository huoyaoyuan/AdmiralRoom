using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Huoyaoyuan.AdmiralRoom.Officer.Battle;

namespace Huoyaoyuan.AdmiralRoom.Controls
{
    class WinRankText : ContentControl
    {
        private readonly TextBlock text = new TextBlock();
        public WinRankText()
        {
            Content = text;
        }

        public WinRank WinRank
        {
            get { return (WinRank)GetValue(WinRankProperty); }
            set { SetValue(WinRankProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WinRank.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WinRankProperty =
            DependencyProperty.Register(nameof(WinRank), typeof(WinRank), typeof(WinRankText), new PropertyMetadata(OnWinRankChanged));

        private static void OnWinRankChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var t = d as WinRankText;
            if (e.NewValue is WinRank winrank)
                t.SetContent(winrank);
            else t.Clear();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetContent(this.WinRank);
        }

        private void SetContent(WinRank winrank)
        {
            switch (winrank)
            {
                case WinRank.Perfect:
                    text.Foreground = Brushes.Gold;
                    ResourceService.SetStringTableReference(text, TextBlock.TextProperty, "Battle_WinRank_Perfect");
                    break;
                case WinRank.S:
                    text.Foreground = Brushes.Gold;
                    ResourceService.SetStringTableReference(text, TextBlock.TextProperty, "Battle_WinRank_S");
                    break;
                case WinRank.A:
                    text.Foreground = Brushes.Red;
                    ResourceService.SetStringTableReference(text, TextBlock.TextProperty, "Battle_WinRank_A");
                    break;
                case WinRank.B:
                    text.Foreground = Brushes.Orange;
                    ResourceService.SetStringTableReference(text, TextBlock.TextProperty, "Battle_WinRank_B");
                    break;
                case WinRank.C:
                    text.Foreground = Brushes.Magenta;
                    ResourceService.SetStringTableReference(text, TextBlock.TextProperty, "Battle_WinRank_C");
                    break;
                case WinRank.D:
                    text.Foreground = Brushes.Green;
                    ResourceService.SetStringTableReference(text, TextBlock.TextProperty, "Battle_WinRank_D");
                    break;
                case WinRank.E:
                    text.Foreground = Brushes.Blue;
                    ResourceService.SetStringTableReference(text, TextBlock.TextProperty, "Battle_WinRank_E");
                    break;
                default:
                    this.Clear();
                    break;
            }
        }

        private void Clear()
        {
            text.ClearValue(ForegroundProperty);
            text.ClearValue(TextBlock.TextProperty);
        }
    }
}
