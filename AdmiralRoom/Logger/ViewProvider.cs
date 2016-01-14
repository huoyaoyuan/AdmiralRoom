using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    class ViewProvider<T> : NotificationObject, IUpdatable
    {
        public class Column : NotifySourceObject<ViewProvider<T>>
        {
            public Type MemberType { get; set; }
            public string MemberName { get; set; }
            public MethodInfo MemberGetter { get; set; }
            public string Title { get; set; }
            public string TitleKey => "LogTitle_" + Title;
            public string FullTitleKey => "Resources" + TitleKey;
            public bool IsFilter { get; set; }
            public string[] Values { get; }

            #region SelectedValue
            private string _selectedvalue;
            public string SelectedValue
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
                var input = Expression.Parameter(MemberType);
                var getmember = Expression.Property(input, MemberGetter);
                var value = Convert.ChangeType(SelectedValue, MemberType);
                var valueexp = Expression.Constant(value, MemberType);
                var compare = Expression.Equal(getmember, valueexp);
                var expression = Expression.Lambda<Func<T, bool>>(compare, input);
                Selector = expression.Compile();
            }
        }
        public Column[] Columns { get; }
        public GridViewColumn[] ViewColumns { get; }
        public readonly Logger<T> Logger;
        private readonly T[] readed;

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

        public ViewProvider(Logger<T> logger)
        {
            Type type = typeof(T);
            Logger = logger;
            Columns = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => Attribute.IsDefined(x, typeof(ShowAttribute)))
                .Select(x =>
                {
                    var attr = Attribute.GetCustomAttribute(x, typeof(ShowAttribute)) as ShowAttribute;
                    return new Column
                    {
                        Title = attr.Title ?? x.Name,
                        IsFilter = attr.IsFilter,
                        MemberName = x.Name,
                        MemberGetter = x.GetMethod,
                        MemberType = x.PropertyType,
                        Source = this
                    };
                })
                .ToArray();
            ViewColumns = Columns.Select(x =>
            {
                var column = new GridViewColumn
                {
                    DisplayMemberBinding = new Binding(x.MemberName)
                };
                BindingOperations.SetBinding(column, GridViewColumn.HeaderProperty, new Views.Extensions.LocalizableExtension(x.TitleKey));
                return column;
            }).ToArray();
            readed = logger.Read().ToArray();
            Update();
        }
        public void Update()
        {
            IEnumerable<T> logs = readed;
            if (logs == null) return;
            foreach (var column in Columns)
                logs = logs.Where(column.Selector);
            Displayed = logs.ToArray();
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal sealed class ShowAttribute : Attribute
    {
        public string Title { get; }
        public bool IsFilter { get; set; }
        public ShowAttribute(string title = null) { Title = title; }
    }
}
