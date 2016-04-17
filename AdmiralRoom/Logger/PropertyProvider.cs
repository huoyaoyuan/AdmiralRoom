using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public sealed class PropertyProvider<T>
    {
        private readonly Func<T, string[]> getter;
        private readonly Func<string[], T> setter;
        public string[] Titles { get; }
        public PropertyProvider(bool useshown = false)
        {
            Type type = typeof(T);
            PropertyInfo[] prop;
            if (useshown)
            {
                prop = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => Attribute.IsDefined(x, typeof(ShowAttribute)))
                    .ToArray();
                Titles = prop.Select(x => (Attribute.GetCustomAttribute(x, typeof(ShowAttribute)) as ShowAttribute).Title).ToArray();
            }
            else
            {
                prop = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => Attribute.IsDefined(x, typeof(LogAttribute)))
                    .ToArray();
                Titles = prop.Select(x => x.Name).ToArray();
            }
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
                ChangeTypeHelper(Expression.ArrayIndex(setinput, Expression.Constant(i)), x.PropertyType));
            var members = converters.Zip(prop, (x, y) => Expression.Bind(y, x));
            var init = Expression.MemberInit(Expression.New(typeof(T)), members);
            var setterexpression = Expression.Lambda<Func<string[], T>>(init, setinput);
            setter = setterexpression.Compile();
        }
        public string[] GetValues(T item) => getter(item);
        public T GetItem(string[] values) => setter(values);
        private static Expression ChangeTypeHelper(Expression input, Type type)
        {
            if (type == typeof(string)) return input;
            MethodInfo parse;
            if ((parse = type.GetMethod("Parse", new[] { typeof(string) }))?.ReturnType == type)
                return Expression.Call(parse, input);
            Expression convert;
            if (type.IsEnum)
            {
                var parser = EnumParseHelper.GetParser(type);
                convert = Expression.Invoke(parser, input);
            }
            else
            {
                parse = typeof(Convert).GetMethod(nameof(Convert.ChangeType), new[] { typeof(object), typeof(Type) });
                convert = Expression.Call(parse, input, Expression.Constant(type));
            }
            return Expression.Convert(convert, type);
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal sealed class LogAttribute : Attribute { }
}
