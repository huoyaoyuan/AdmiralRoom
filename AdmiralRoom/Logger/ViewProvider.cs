using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public class ViewProvider<T> : NotificationObject, IUpdatable
    {
        internal class Column : NotifySourceObject<ViewProvider<T>>
        {
            public Type MemberType { get; set; }
            public string MemberName { get; set; }
            public string TitleKey { get; set; }
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
                if (!Enable)
                {
                    Selector = _ => true;
                    return;
                }
                var input = Expression.Parameter(MemberType);
                var getmember = Expression.PropertyOrField(input, MemberName);
                var value = Convert.ChangeType(SelectedValue, MemberType);
                var valueexp = Expression.Constant(value, MemberType);
                var compare = Expression.Equal(getmember, valueexp);
                var expression = Expression.Lambda<Func<T, bool>>(compare, input);
                Selector = expression.Compile();
            }
        }
        public IEnumerable<T> Displayed { get; private set; }

        public void Update()
        {

        }
    }
}
