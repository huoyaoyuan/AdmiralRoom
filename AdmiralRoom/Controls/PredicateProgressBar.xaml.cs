using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Huoyaoyuan.AdmiralRoom.Controls
{
    /// <summary>
    /// PredicateProgressBar.xaml 的交互逻辑
    /// </summary>
    public partial class PredicateProgressBar : UserControl
    {
        public PredicateProgressBar()
        {
            InitializeComponent();
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(PredicateProgressBar), new PropertyMetadata(0.0));



        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(PredicateProgressBar), new PropertyMetadata(0.0));



        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(PredicateProgressBar), new PropertyMetadata(100.0));



        public double PredictValue
        {
            get { return (double)GetValue(PredictValueProperty); }
            set { SetValue(PredictValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PredictValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PredictValueProperty =
            DependencyProperty.Register("PredictValue", typeof(double), typeof(PredicateProgressBar), new PropertyMetadata(0.0));



        public Brush PredictForeground
        {
            get { return (Brush)GetValue(PredictForegroundProperty); }
            set { SetValue(PredictForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PredictForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PredictForegroundProperty =
            DependencyProperty.Register("PredictForeground", typeof(Brush), typeof(PredicateProgressBar), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));


    }
}
