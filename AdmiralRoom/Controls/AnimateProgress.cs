using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace Huoyaoyuan.AdmiralRoom.Controls
{
    [TemplatePart(Name = "PART_Progress",Type = typeof(RangeBase))]
    public class AnimateProgress : RangeBase
    {
        static AnimateProgress()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimateProgress), new FrameworkPropertyMetadata(typeof(AnimateProgress)));
            MaximumProperty.OverrideMetadata(typeof(AnimateProgress), new FrameworkPropertyMetadata(100.0));
        }
        private RangeBase PART_Progress;
        
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            var animation = new DoubleAnimation()
            {
                From = oldValue,
                To = newValue,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
            };
            PART_Progress?.BeginAnimation(ValueProperty, animation);
        }
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Progress = GetTemplateChild("PART_Progress") as RangeBase;
            if (PART_Progress != null)
                PART_Progress.Value = Value;
        }
    }
}
