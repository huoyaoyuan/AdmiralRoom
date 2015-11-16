using System;
using System.Collections.Generic;

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
    }
}
