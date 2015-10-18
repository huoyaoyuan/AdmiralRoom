using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Huoyaoyuan.AdmiralRoom.Controls
{
    /// <summary>
    /// AnimateProgress.xaml 的交互逻辑
    /// </summary>
    public partial class AnimateProgress : UserControl
    {
        public AnimateProgress()
        {
            InitializeComponent();
        }


        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(AnimateProgress), new PropertyMetadata(0.0));



        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(AnimateProgress), new PropertyMetadata(100.0));



        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(AnimateProgress), new PropertyMetadata(0.0, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as AnimateProgress;
            var animation = new DoubleAnimation()
            {
                From = (double)e.OldValue,
                To = (double)e.NewValue,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
            };
            control.bar.BeginAnimation(ProgressBar.ValueProperty, animation);
        }
    }
}
