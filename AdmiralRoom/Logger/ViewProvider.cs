using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    internal class ViewProvider<T> : ViewProviderBase, IUpdatable
        where T : ILog
    {
        public class Column : NotifySourceObject<ViewProvider<T>>
        {
            public Type MemberType { get; set; }
            public string MemberName { get; set; }
            public MethodInfo MemberGetter { get; set; }
            public MethodInfo FilterGetter { get; set; }
            public string Title { get; set; }
            public string FullTitleKey => "LogTitle_" + Title;
            public object[] Values
            {
                get
                {
                    var input = Expression.Parameter(typeof(T));
                    var getmember = Expression.Property(input, MemberGetter);
                    var obj = Expression.Convert(getmember, typeof(object));
                    var selector = Expression.Lambda<Func<T, object>>(obj, input).Compile();
                    var filterselector = selector;
                    if (FilterGetter != null)
                    {
                        var getfilter = Expression.Property(input, FilterGetter);
                        var obj2 = Expression.Convert(getfilter, typeof(object));
                        filterselector = Expression.Lambda<Func<T, object>>(obj2, input).Compile();
                    }
                    return Source.readed.GroupBy(filterselector).OrderBy(x => x.Key).Select(x => selector(x.First())).Distinct().ToArray();
                }
            }

            #region SelectedValue
            private object _selectedvalue;
            public object SelectedValue
            {
                get { return _selectedvalue; }
                set
                {
                    if (_selectedvalue != value)
                    {
                        _selectedvalue = value;
                        RefreshSelector();
                        OnPropertyChanged();
                    }
                }
            }
            #endregion

            #region Enable
            private bool _enable;
            public bool Enable
            {
                get { return _enable; }
                set
                {
                    if (_enable != value)
                    {
                        _enable = value;
                        RefreshSelector();
                        OnPropertyChanged();
                    }
                }
            }
            #endregion

            public Func<T, bool> Selector { get; private set; } = _ => true;
            private void RefreshSelector()
            {
                if (!Enable || SelectedValue == null)
                {
                    Selector = _ => true;
                    return;
                }
                var input = Expression.Parameter(typeof(T));
                var getmember = Expression.Property(input, MemberGetter);
                var value = Convert.ChangeType(SelectedValue, MemberType);
                var valueexp = Expression.Constant(value, MemberType);
                var compare = Expression.Equal(getmember, valueexp);
                var expression = Expression.Lambda<Func<T, bool>>(compare, input);
                Selector = expression.Compile();
            }
        }
        public Column[] Columns { get; }
        public readonly Logger<T> Logger;
        private readonly T[] readed;
        public int TotalCount { get; }

        #region Displayed
        private IReadOnlyCollection<T> _displayed;
        public IReadOnlyCollection<T> Displayed
        {
            get { return _displayed; }
            set
            {
                if (_displayed != value)
                {
                    _displayed = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region TimeFrom
        private DateTime _timefrom = DateTime.Today.AddDays(-6);
        public DateTime TimeFrom
        {
            get { return _timefrom; }
            set
            {
                if (_timefrom != value)
                {
                    _timefrom = value;
                    OnPropertyChanged();
                    Update();
                }
            }
        }
        #endregion

        #region TimeTo
        private DateTime _timeto = DateTime.Today;
        public DateTime TimeTo
        {
            get { return _timeto; }
            set
            {
                if (_timeto != value)
                {
                    _timeto = value;
                    OnPropertyChanged();
                    Update();
                }
            }
        }
        #endregion

        public bool IsCustomTimeRange => SelectedTimeRange == null;

        #region SelectedTimeRange
        private TimeSpan? _selectedtimerange = TimeSpan.FromDays(1);
        public TimeSpan? SelectedTimeRange
        {
            get { return _selectedtimerange; }
            set
            {
                if (_selectedtimerange != value)
                {
                    _selectedtimerange = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsCustomTimeRange));
                    Update();
                }
            }
        }
        #endregion

        public class TimeRange
        {
            public string Title { get; set; }
            public string TitleKey => "TimeRange_" + Title;
            public TimeSpan? Time { get; set; }
        }
        public TimeRange[] TimeRanges { get; } =
        {
            new TimeRange { Title = "Hours1", Time = TimeSpan.FromHours(1) },
            new TimeRange { Title = "Hours6", Time = TimeSpan.FromHours(6) },
            new TimeRange { Title = "Hours12", Time = TimeSpan.FromHours(12) },
            new TimeRange { Title = "Hours24", Time = TimeSpan.FromDays(1) },
            new TimeRange { Title = "Days3", Time = TimeSpan.FromDays(3) },
            new TimeRange { Title = "Days7", Time = TimeSpan.FromDays(7) },
            new TimeRange { Title = "All", Time = TimeSpan.MaxValue },
            new TimeRange { Title = "Custom", Time = null }
        };
        public ViewProvider(Logger<T> logger)
        {
            Type type = typeof(T);
            Logger = logger;
            var prop = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Columns = prop.Where(x => Attribute.IsDefined(x, typeof(FilterAttribute)))
                .Select(x => new Column
                {
                    Title = (Attribute.GetCustomAttribute(x, typeof(ShowAttribute)) as ShowAttribute)?.Title ?? x.Name,
                    MemberName = x.Name,
                    MemberGetter = x.GetMethod,
                    MemberType = x.PropertyType,
                    FilterGetter = FindFilter((Attribute.GetCustomAttribute(x, typeof(FilterAttribute)) as FilterAttribute).Filter),
                    Source = this
                })
                .ToArray();
            ViewColumns = prop.Where(x => Attribute.IsDefined(x, typeof(ShowAttribute)))
                .Select(x =>
                {
                    var column = new GridViewColumn
                    {
                        DisplayMemberBinding = new Binding(x.Name)
                    };
                    ResourceService.SetStringTableBinding(column, GridViewColumn.HeaderProperty,
                        "LogTitle_" + ((Attribute.GetCustomAttribute(x, typeof(ShowAttribute)) as ShowAttribute).Title ?? x.Name));
                    return column;
                }).ToArray();
            readed = logger.Read().ToArray();
            TotalCount = readed.Length;
            Update();
        }
        public void Update()
        {
            IEnumerable<T> logs = readed;
            if (logs == null) return;
            var now = DateTime.UtcNow;
            var nextday = TimeTo.AddDays(1);
#pragma warning disable CC0014 // Use ternary operator
            if (SelectedTimeRange != null)
#pragma warning restore CC0014 // Use ternary operator
                logs = logs.Where(x => now - x.DateTime <= SelectedTimeRange);
            else logs = logs.Where(x => x.DateTime.ToLocalTime() >= TimeFrom && x.DateTime.ToLocalTime() <= nextday);
            foreach (var column in Columns)
                logs = logs.Where(column.Selector);
            Displayed = logs.ToArray();
        }
        private static MethodInfo FindFilter(string filter)
        {
            if (filter == null) return null;
            return typeof(T).GetProperty(filter).GetMethod;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal sealed class ShowAttribute : Attribute
    {
        public string Title { get; }
        public ShowAttribute(string title = null) { Title = title; }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal sealed class FilterAttribute : Attribute
    {
        public string Filter { get; }
        public FilterAttribute(string filter = null) { Filter = filter; }
    }
}
