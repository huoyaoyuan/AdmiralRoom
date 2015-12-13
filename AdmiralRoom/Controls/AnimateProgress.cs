using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace Huoyaoyuan.AdmiralRoom.Controls
{
    [TemplatePart(Name = "PART_Progress", Type = typeof(RangeBase))]
    public class AnimateProgress : RangeBase
    {
        static AnimateProgress()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimateProgress), new FrameworkPropertyMetadata(typeof(AnimateProgress)));
            MaximumProperty.OverrideMetadata(typeof(AnimateProgress), new FrameworkPropertyMetadata(100.0));
        }
        private RangeBase PART_Progress;

        public InitAnimateFrom InitAnimateFrom
        {
            get { return (InitAnimateFrom)GetValue(InitAnimateFromProperty); }
            set { SetValue(InitAnimateFromProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InitAnimateFrom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InitAnimateFromProperty =
            DependencyProperty.Register("InitAnimateFrom", typeof(InitAnimateFrom), typeof(AnimateProgress), new PropertyMetadata(InitAnimateFrom.None));

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            var animation = new DoubleAnimation
            {
                From = oldValue,
                To = newValue,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
            };
            PART_Progress?.BeginAnimation(ValueProperty, animation);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Progress = GetTemplateChild("PART_Progress") as RangeBase;
            if (PART_Progress != null)
                switch (InitAnimateFrom)
                {
                    case InitAnimateFrom.None:
                        PART_Progress.Value = Value;
                        break;
                    case InitAnimateFrom.Minimum:
                        PART_Progress.BeginAnimation(ValueProperty, new DoubleAnimation
                        {
                            From = Minimum,
                            To = Value,
                            Duration = TimeSpan.FromSeconds(1),
                            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
                        });
                        break;
                    case InitAnimateFrom.Maximum:
                        PART_Progress.BeginAnimation(ValueProperty, new DoubleAnimation
                        {
                            From = Maximum,
                            To = Value,
                            Duration = TimeSpan.FromSeconds(1),
                            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
                        });
                        break;
                }
        }
    }
    public enum InitAnimateFrom { None, Minimum, Maximum }
}
