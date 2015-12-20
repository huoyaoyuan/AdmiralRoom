using System.Windows;
using System.Windows.Controls;
using Huoyaoyuan.AdmiralRoom.Officer;

namespace Huoyaoyuan.AdmiralRoom.Controls
{
    public class ExpBar : Control
    {
        static ExpBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExpBar), new FrameworkPropertyMetadata(typeof(ExpBar)));
        }

        public Exp Exp
        {
            get { return (Exp)GetValue(ExpProperty); }
            set { SetValue(ExpProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Exp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpProperty =
            DependencyProperty.Register("Exp", typeof(Exp), typeof(ExpBar), new PropertyMetadata(null));

        public int Level
        {
            get { return (int)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Level.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.Register("Level", typeof(int), typeof(ExpBar), new PropertyMetadata(0));
    }
}
