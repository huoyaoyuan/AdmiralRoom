using System.Windows;
using System.Windows.Controls;
using Huoyaoyuan.AdmiralRoom.Models;

namespace Huoyaoyuan.AdmiralRoom.Controls
{
    /// <summary>
    /// ExpBar.xaml 的交互逻辑
    /// </summary>
    public partial class ExpBar : UserControl
    {
        public ExpBar()
        {
            InitializeComponent();
        }

        public Exp Exp
        {
            get { return (Exp)GetValue(ExpProperty); }
            set { SetValue(ExpProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Exp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpProperty =
            DependencyProperty.Register("Exp", typeof(Exp), typeof(ExpBar), new PropertyMetadata(OnExpChange));

        private static void OnExpChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ExpBar;
            Exp exp = e.NewValue as Exp ?? new Exp();
            control.ToolTipText.Text = exp.ToString();
            if (exp.Percent == -1)
            {
                int thislevelexp = ConstData.GetAdmiralExp(control.Level);
                double levelexp = exp.NextLevel - thislevelexp;
                double rightper = exp.Next / levelexp;
                double leftper = 1 - rightper; ;
                if (rightper <= 0 && leftper <= 0)
                {
                    leftper = 0;
                    rightper = 1;
                }
                control.BarExp.Minimum = thislevelexp;
                control.BarExp.Maximum = exp.NextLevel;
                control.BarExp.Value = exp.Current;
            }
            else
            {
                control.BarExp.Minimum = 0;
                control.BarExp.Maximum = 100;
                control.BarExp.Value = exp.Percent;
            }
        }

        public int Level
        {
            get { return (int)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Level.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.Register("Level", typeof(int), typeof(ExpBar), new PropertyMetadata());
    }
}
