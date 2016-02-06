using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Huoyaoyuan.AdmiralRoom.Logger;

namespace Huoyaoyuan.AdmiralRoom.Views.Standalone
{
    public class MaterialChart : FrameworkElement
    {
        public IEnumerable<MaterialLog> Source
        {
            get { return (IEnumerable<MaterialLog>)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(IEnumerable<MaterialLog>), typeof(MaterialChart), new PropertyMetadata(null, ReRender));

        public TimeSpan During
        {
            get { return (TimeSpan)GetValue(DuringProperty); }
            set { SetValue(DuringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for During.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DuringProperty =
            DependencyProperty.Register(nameof(During), typeof(TimeSpan), typeof(MaterialChart), new PropertyMetadata(TimeSpan.FromDays(1), ReRender));

        private static void ReRender(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((FrameworkElement)d).InvalidateVisual();

        private readonly DateTime now = DateTime.UtcNow;
        protected override void OnRender(DrawingContext drawingContext)
        {
            var black = new SolidColorBrush(Colors.Black).TryFreeze();
            var basetext = new FormattedText("00-00 00:00", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface(""), 14, black);
            double left = basetext.Width;
            double top = basetext.Height / 2;
            double chartwidth = ActualWidth - basetext.Width * 2 - 8;
            double chartheight = ActualHeight - basetext.Height * 2;
            var gray1 = new Pen(new SolidColorBrush(Colors.LightGray), 1).TryFreeze();
            var gray2 = new Pen(new SolidColorBrush(Colors.LightGray), 2).TryFreeze();

            //横线
            drawingContext.DrawLine(gray1, new Point(left - 4, top), new Point(left + 4 + chartwidth, top));
            drawingContext.DrawLine(gray1, new Point(left - 4, top + chartheight * .25), new Point(left + 4 + chartwidth, top + chartheight * .25));
            drawingContext.DrawLine(gray1, new Point(left - 4, top + chartheight * .5), new Point(left + 4 + chartwidth, top + chartheight * .5));
            drawingContext.DrawLine(gray1, new Point(left - 4, top + chartheight * .75), new Point(left + 4 + chartwidth, top + chartheight * .75));
            drawingContext.DrawLine(gray2, new Point(left - 4, top + chartheight), new Point(left + 4 + chartwidth, top + chartheight));

            //纵线
            drawingContext.DrawLine(gray2, new Point(left, top), new Point(left, top + chartheight));
            drawingContext.DrawLine(gray2, new Point(left + chartwidth, top), new Point(left + chartwidth, top + chartheight));

            if (Source == null) return;

        }
    }
}
