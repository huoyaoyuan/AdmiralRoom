using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    internal sealed class PropertyProvider<T>
    {
        private readonly Func<T, string[]> getter;
        public string[] TextKeys { get; }
        public string[] Titles { get; }
        public PropertyProvider()
        {
            Type type = typeof(T);
            PropertyInfo[] prop = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanRead)
                .Where(x => Attribute.IsDefined(x, typeof(LogAttribute)))
                .ToArray();
            int count = prop.Length;
            Titles = prop.Select(x => ((LogAttribute)Attribute.GetCustomAttribute(x, typeof(LogAttribute))).Title ?? x.Name).ToArray();
            TextKeys = prop.Select(x => ((LogAttribute)Attribute.GetCustomAttribute(x, typeof(LogAttribute))).TextKey ?? x.Name).ToArray();
            var input = Expression.Parameter(type);
            MethodInfo tostring = typeof(object).GetMethod(nameof(ToString));
            var getters = prop.Select(x => Expression.Property(input, x))
                .Select(x => Expression.Call(x, tostring));
            var array = Expression.NewArrayInit(typeof(string), getters);
            var expression = Expression.Lambda<Func<T, string[]>>(array, input);
            getter = expression.Compile();
        }
        public string[] GetValues(T item) => getter(item);
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
