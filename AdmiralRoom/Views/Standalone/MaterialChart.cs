using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Huoyaoyuan.AdmiralRoom.Logger;
using static System.Math;
using static Huoyaoyuan.AdmiralRoom.CollectionEx;

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
        private bool[] shown;
        private int highlight = -1;
        protected override void OnRender(DrawingContext drawingContext)
        {
            var black = new SolidColorBrush(Colors.Black).TryFreeze();
            var typeface = new Typeface("");
            const double fontsize = 14;
            var text = new FormattedText("00-00 00:00", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            double left = text.Width / 2 + 16;
            double top = text.Height / 2;
            double chartwidth = ActualWidth - text.Width - 8 - 16;
            double chartheight = ActualHeight - text.Height * 2;
            var gray1 = new Pen(new SolidColorBrush(Colors.LightGray), 1).TryFreeze();
            var gray2 = new Pen(new SolidColorBrush(Colors.LightGray), 2).TryFreeze();

            drawingContext.DrawLine(gray1, new Point(left - 4, top), new Point(left + 4 + chartwidth, top));
            drawingContext.DrawLine(gray1, new Point(left - 4, top + chartheight * .25), new Point(left + 4 + chartwidth, top + chartheight * .25));
            drawingContext.DrawLine(gray1, new Point(left - 4, top + chartheight * .5), new Point(left + 4 + chartwidth, top + chartheight * .5));
            drawingContext.DrawLine(gray1, new Point(left - 4, top + chartheight * .75), new Point(left + 4 + chartwidth, top + chartheight * .75));
            drawingContext.DrawLine(gray2, new Point(left - 4, top + chartheight), new Point(left + 4 + chartwidth, top + chartheight));

            drawingContext.DrawLine(gray2, new Point(left, top), new Point(left, top + chartheight));
            drawingContext.DrawLine(gray2, new Point(left + chartwidth, top), new Point(left + chartwidth, top + chartheight));

            if (Source == null) return;
            int outofdatecount = 0;
            foreach (var log in Source)
            {
                if (now - log.DateTime > During) outofdatecount++;
                else break;
            }
            var recent = Source.Skip(outofdatecount - 1);
            if (recent.IsNullOrEmpty()) return;

            int max1 = recent.Max(x => Max(x.Fuel, x.Bull, x.Steel, x.Bauxite));
            int min1 = recent.Min(x => Min(x.Fuel, x.Bull, x.Steel, x.Bauxite));
            int max2 = recent.Max(x => Max(x.InstantBuild, x.InstantRepair, x.Development, x.Improvement));
            int min2 = recent.Min(x => Min(x.InstantBuild, x.InstantRepair, x.Development, x.Improvement));
            min1 = (int)Floor(min1 / 5000.0) * 5000;
            max1 = (int)Ceiling((max1 - min1) / 1000.0) * 1000 + min1;
            min2 = (int)Floor(min2 / 500.0) * 500;
            max2 = (int)Ceiling((max2 - min2) / 100.0) * 100 + min2;

            text = new FormattedText(max1.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left - 6 - text.Width, 0));
            text = new FormattedText(((max1 * 3 + min1) / 4).ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left - 6 - text.Width, chartheight * .25));
            text = new FormattedText(((max1 + min1) / 2).ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left - 6 - text.Width, chartheight * .5));
            text = new FormattedText(((max1 + min1 * 3) / 4).ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left - 6 - text.Width, chartheight * .75));
            text = new FormattedText(min1.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left - 6 - text.Width, chartheight));

            text = new FormattedText(max2.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left + 6 + chartwidth, 0));
            text = new FormattedText(((max2 * 3 + min2) / 4).ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left + 6 + chartwidth, chartheight * .25));
            text = new FormattedText(((max2 + min2) / 2).ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left + 6 + chartwidth, chartheight * .5));
            text = new FormattedText(((max2 + min2 * 3) / 4).ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left + 6 + chartwidth, chartheight * .75));
            text = new FormattedText(min2.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left + 6 + chartwidth, chartheight));

            text = new FormattedText((now - During).ToLocalTime().ToString("MM-dd HH:mm"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left - text.Width / 2, chartheight + top * 2));
            text = new FormattedText((now.AddSeconds(During.TotalSeconds * -.75)).ToLocalTime().ToString("MM-dd HH:mm"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left - text.Width / 2 + chartwidth * .25, chartheight + top * 2));
            text = new FormattedText((now.AddSeconds(During.TotalSeconds * -.5)).ToLocalTime().ToString("MM-dd HH:mm"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left - text.Width / 2 + chartwidth * .5, chartheight + top * 2));
            text = new FormattedText((now.AddSeconds(During.TotalSeconds * -.25)).ToLocalTime().ToString("MM-dd HH:mm"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left - text.Width / 2 + chartwidth * .75, chartheight + top * 2));
            text = new FormattedText((now).ToLocalTime().ToString("MM-dd HH:mm"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left - text.Width / 2 + chartwidth, chartheight + top * 2));
        }

        public void UpdateShown(bool[] shown, int highlight)
        {
            this.shown = shown;
            this.highlight = highlight;
            ReRender(this, new DependencyPropertyChangedEventArgs());
        }
    }
}
