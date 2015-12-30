using System;
using System.Linq;

namespace Huoyaoyuan.AdmiralRoom
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class MemberwiseCopyIgnoreAttribute : Attribute { }
    static class MemberwiseCopyHelper
    {
        public static void MemberwiseCopy<T>(this T dest, T source)
        {
            Type type = typeof(T);
            type.GetProperties().Where(x => x.CanRead && x.CanWrite)
                .Where(x => x.GetCustomAttributes(typeof(MemberwiseCopyIgnoreAttribute), true).Length == 0)
                .ForEach(x => x.SetValue(dest, x.GetValue(source)));
            type.GetFields()
                .Where(x => x.GetCustomAttributes(typeof(MemberwiseCopyIgnoreAttribute), true).Length == 0)
                .ForEach(x => x.SetValue(dest, x.GetValue(source)));
        }
    }
}
