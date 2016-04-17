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
    internal class MaterialChart : FrameworkElement
    {
        public IEnumerable<MaterialLog> Source
        {
            get { return (IEnumerable<MaterialLog>)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(IEnumerable<MaterialLog>), typeof(MaterialChart), new PropertyMetadata(null, ReRender));

        public DateTime From
        {
            get { return (DateTime)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        // Using a DependencyProperty as the backing store for From.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register(nameof(From), typeof(DateTime), typeof(MaterialChart), new PropertyMetadata(DateTime.UtcNow, ReRender));

        public DateTime To
        {
            get { return (DateTime)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        // Using a DependencyProperty as the backing store for To.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register(nameof(To), typeof(DateTime), typeof(MaterialChart), new PropertyMetadata(DateTime.UtcNow, ReRender));

        private static void ReRender(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = d as MaterialChart;
            chart.InvalidateVisual();
        }

        private bool[] shown = { true, true, true, true, true, true, true, true };
        private int? highlight;

        private int min1, min2, max1, max2;
        private double left, top, chartheight, chartwidth;
        private PathGeometry[] lines = new PathGeometry[8];
        private bool colorsonly;
        protected override void OnRender(DrawingContext drawingContext)
        {
            var black = new SolidColorBrush(Colors.Black).TryFreeze();
            var typeface = new Typeface("");
            const double fontsize = 14;
            var text = new FormattedText("00-00 00:00", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            left = text.Width / 2 + 16;
            top = text.Height / 2;
            chartwidth = ActualWidth - text.Width - 8 - 16;
            chartheight = ActualHeight - text.Height * 2;
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
                if (log.DateTime < From) outofdatecount++;
                else break;
            }
            var recent = Reduce(Source.Skip(outofdatecount - 1)).ToArray();
            if (recent.IsNullOrEmpty()) return;

            max1 = recent.Max(x => Max(x.Fuel, x.Bull, x.Steel, x.Bauxite));
            min1 = recent.Min(x => Min(x.Fuel, x.Bull, x.Steel, x.Bauxite));
            max2 = recent.Max(x => Max(x.InstantBuild, x.InstantRepair, x.Development, x.Improvement));
            min2 = recent.Min(x => Min(x.InstantBuild, x.InstantRepair, x.Development, x.Improvement));
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

            text = new FormattedText(From.ToLocalTime().ToString("MM-dd HH:mm"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left - text.Width / 2, chartheight + top * 2));
            text = new FormattedText((To.AddSeconds((To - From).TotalSeconds * -.75)).ToLocalTime().ToString("MM-dd HH:mm"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left - text.Width / 2 + chartwidth * .25, chartheight + top * 2));
            text = new FormattedText((To.AddSeconds((To - From).TotalSeconds * -.5)).ToLocalTime().ToString("MM-dd HH:mm"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left - text.Width / 2 + chartwidth * .5, chartheight + top * 2));
            text = new FormattedText((To.AddSeconds((To - From).TotalSeconds * -.25)).ToLocalTime().ToString("MM-dd HH:mm"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left - text.Width / 2 + chartwidth * .75, chartheight + top * 2));
            text = new FormattedText(To.ToLocalTime().ToString("MM-dd HH:mm"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontsize, black);
            drawingContext.DrawText(text, new Point(left - text.Width / 2 + chartwidth, chartheight + top * 2));

            if (!colorsonly)
            {
                var following = recent.Skip(1);
                if (following.IsNullOrEmpty())
                {
                    var last = recent.First();
                    following = Enumerable.Repeat(new MaterialLog
                    {
                        Fuel = last.Fuel,
                        Bull = last.Bull,
                        Steel = last.Steel,
                        Bauxite = last.Bauxite,
                        InstantBuild = last.InstantBuild,
                        InstantRepair = last.InstantRepair,
                        Development = last.Development,
                        Improvement = last.Improvement,
                        DateTime = To
                    }, 1);
                }
                for (int i = 0; i < 8; i++)
                {
                    var first = recent.First();
                    var next = following.First();
                    Point firstpoint;
                    firstpoint = first.DateTime < From ?
                        MakeChartPoint(From,
                            (first.TakeValue(i + 1) * (next.DateTime - From).TotalSeconds
                            + next.TakeValue(i + 1) * (From - first.DateTime).TotalSeconds)
                            / (next.DateTime - first.DateTime).TotalSeconds,
                            i + 1) :
                        MakeChartPoint(first.DateTime, first.TakeValue(i + 1), i + 1);
                    var figure = new PathFigure(firstpoint,
                        following.Select(x => new LineSegment(MakeChartPoint(x.DateTime, x.TakeValue(i + 1), i + 1), true)),
                        false);
                    lines[i] = new PathGeometry(new[] { figure });
                }
            }
            for (int i = 0; i < 8; i++)
            {
                if (!shown[i]) continue;
                if (highlight == i) continue;
                var brush = FindResource("MaterialColor" + (i + 1)) as SolidColorBrush;
                Color c = brush.Color;
                Pen pen;
                pen = highlight != null ?
                    new Pen(new SolidColorBrush(Color.Multiply(c, 0.5f)), 1) :
                    new Pen(brush, 2);
                drawingContext.DrawGeometry(null, pen, lines[i]);
            }
            if (highlight != null)
                drawingContext.DrawGeometry(null,
                    new Pen(FindResource("MaterialColor" + (highlight + 1)) as SolidColorBrush, 2),
                    lines[highlight.Value]);
            colorsonly = false;
        }

        private Point MakeChartPoint(DateTime datetime, double value, int id)
        {
            int max, min;
            if (id <= 4)//major
            {
                max = max1;
                min = min1;
            }
            else
            {
                max = max2;
                min = min2;
            }
            return new Point(chartwidth * (datetime - From).TotalSeconds / (To - From).TotalSeconds + left,
                chartheight * (max - value) / (max - min) + top);
        }

        public void UpdateShown(bool[] shown, int? highlight)
        {
            this.shown = shown;
            this.highlight = highlight;
            if (highlight != null && !shown[highlight.Value]) this.highlight = null;

            colorsonly = true;
            this.InvalidateVisual();
        }

        private IEnumerable<MaterialLog> Reduce(IEnumerable<MaterialLog> source)
        {
            double span = (To - From).TotalSeconds / chartwidth;
            MaterialLog prev = null;
            foreach (var log in source)
                if (prev == null) prev = log;
                else
                {
                    if ((int)((prev.DateTime - From).TotalSeconds / span) !=
                        (int)((log.DateTime - From).TotalSeconds / span))
                        yield return prev;
                    prev = log;
                }
            if (prev != null) yield return prev;
        }
    }
}
