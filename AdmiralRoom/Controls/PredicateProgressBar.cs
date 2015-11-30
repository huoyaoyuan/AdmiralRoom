using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Huoyaoyuan.AdmiralRoom.Controls
{
    public class PredicateProgressBar : RangeBase
    {
        static PredicateProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PredicateProgressBar), new FrameworkPropertyMetadata(typeof(PredicateProgressBar)));
            MaximumProperty.OverrideMetadata(typeof(PredicateProgressBar), new FrameworkPropertyMetadata(100.0));
        }

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
            DependencyProperty.Register("PredictForeground", typeof(Brush), typeof(PredicateProgressBar), new PropertyMetadata(new SolidColorBrush(Colors.Aqua)));
    }
}
