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
        public static T With<T>(this T source, Action<T> action)
            where T : class
        {
            action(source);
            return source;
        }
        public static bool HasItem<T>(this IEnumerable<T> source) => source?.Any() ?? false;
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) => !source.HasItem();
        public static IEnumerable<T> ConcatNotNull<T>(this IEnumerable<T> source, IEnumerable<T> param)
            => param != null ? source.Concat(param) : source;
        public static int Max(int a, params int[] array)
        {
            int max = a;
            foreach (int x in array)
                if (x > max) max = x;
            return max;
        }
        public static int Min(int a, params int[] array)
        {
            int min = a;
            foreach (int x in array)
                if (x < min) min = x;
            return min;
        }
    }
}
