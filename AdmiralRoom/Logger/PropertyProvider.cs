using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    internal sealed class PropertyProvider<T>
    {
        private readonly Func<T, string[]> getter;
        private readonly Func<string[], T> setter;
        public string[] TextKeys { get; }
        public string[] Titles { get; }
        public PropertyProvider()
        {
            Type type = typeof(T);
            PropertyInfo[] prop = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => Attribute.IsDefined(x, typeof(LogAttribute)))
                .ToArray();
            Titles = prop.Select(x => ((LogAttribute)Attribute.GetCustomAttribute(x, typeof(LogAttribute))).Title ?? x.Name).ToArray();
            TextKeys = prop.Select(x => ((LogAttribute)Attribute.GetCustomAttribute(x, typeof(LogAttribute))).TextKey ?? x.Name).ToArray();
            var getinput = Expression.Parameter(type);
            MethodInfo tostring = typeof(object).GetMethod(nameof(ToString));
            var getters = prop.Select(x => Expression.Property(getinput, x))
                .Select(x => Expression.Call(x, tostring));
            var array = Expression.NewArrayInit(typeof(string), getters);
            var getterexpression = Expression.Lambda<Func<T, string[]>>(array, getinput);
            getter = getterexpression.Compile();

            var setinput = Expression.Parameter(typeof(string[]));
            MethodInfo changetype = typeof(Convert).GetMethod(nameof(Convert.ChangeType), new[] { typeof(object), typeof(Type) });
            var converters = prop.Select((x, i) =>
                Expression.Convert(Expression.Call(changetype,
                                                    Expression.ArrayIndex(setinput, Expression.Constant(i)),
                                                    Expression.Constant(x.PropertyType)),
                    x.PropertyType));
            var members = converters.Zip(prop, (x, y) => Expression.Bind(y, x));
            var init = Expression.MemberInit(Expression.New(typeof(T)), members);
            var setterexpression = Expression.Lambda<Func<string[], T>>(init, setinput);
            setter = setterexpression.Compile();
        }
        public string[] GetValues(T item) => getter(item);
        public T GetItem(string[] values) => setter(values);
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal sealed class LogAttribute : Attribute
    {
        public string Title { get; set; }
        public string TextKey { get; set; }
        public LogAttribute() { }
        public LogAttribute(string title) { Title = title; }
    }
}
