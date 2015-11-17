using System;
using System.Collections.Generic;
using System.Linq;

namespace Huoyaoyuan.AdmiralRoom
{
    static class CollectionEx
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> func)
        {
            foreach (T s in source)
                func(s);
        }
        public static T First<T>(this T[] array) => array[0];
        public static T Last<T>(this T[] array) => array[array.Length - 1];
        public static bool HasItem<T>(this IEnumerable<T> source) => source?.Any() ?? false;
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) => !source.HasItem();
    }
}
