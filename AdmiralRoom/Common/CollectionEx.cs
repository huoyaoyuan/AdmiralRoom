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
        public static void ArrayZip<T1, T2>(this T1[] source, T2[] param, int from, Action<T1, T2> func)
        {
            for (int i = 0; i < source.Length && i + from < param.Length; i++)
                func(source[i], param[from + i]);
        }
        public static void ZipEach<T1, T2>(this IEnumerable<T1> source, IEnumerable<T2> param, Action<T1, T2> func)
        {
            using (var enum1 = source.GetEnumerator())
            using (var enum2 = param.GetEnumerator())
                while (enum1.MoveNext() && enum2.MoveNext())
                    func(enum1.Current, enum2.Current);
        }
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
