using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    class ViewProvider<T> : NotificationObject, IUpdatable
    {
        public class Column : NotifySourceObject<ViewProvider<T>>
        {
            public Type MemberType { get; set; }
            public MethodInfo MemberGetter { get; set; }
            public string Title { get; set; }
            public string TitleKey => "Resources.LogTitle_" + Title;
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
        public IReadOnlyList<T> Displayed { get; private set; }
        private readonly Logger<T> _logger;
        public ViewProvider(Logger<T> logger)
        {
            _logger = logger;
            Type type = typeof(T);
            Columns = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => Attribute.IsDefined(x, typeof(ShowAttribute)))
                .Select(x => new Column
                {
                    Title = (Attribute.GetCustomAttribute(x, typeof(ShowAttribute)) as ShowAttribute).Title ?? x.Name,
                    MemberGetter = x.GetMethod,
                    MemberType = x.PropertyType,
                    Source = this
                })
                .ToArray();
            Update();
        }
        public void Update()
        {

        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal sealed class ShowAttribute : Attribute
    {
        public string Title { get; set; }
        public ShowAttribute(string title) { Title = title; }
    }
}
