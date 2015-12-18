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
        public static void ArrayZip<T1, T2>(this T1[] source, T2[] param, int from, Action<T1, T2> func)
        {
            for (int i = 0; i < source.Length && i + from < param.Length; i++)
                func(source[i], param[from + i]);
        }
        public static void Zip<T1, T2>(this IEnumerable<T1> source, IEnumerable<T2> param, Action<T1, T2> func)
        {
            using (var enum1 = source.GetEnumerator())
            using (var enum2 = param.GetEnumerator())
                while (enum1.MoveNext() && enum2.MoveNext())
                    func(enum1.Current, enum2.Current);
        }
        public static IEnumerable<T> ConcatNotNull<T>(this IEnumerable<T> source, IEnumerable<T> param)
            => param != null ? source.Concat(param) : source;
        public static T[] ArrayNew<T>(int count)
            where T : class, new()
        {
            T[] array = new T[count];
            for (int i = 0; i < count; i++)
                array[i] = new T();
            return array;
        }
        public static T TakeSingle<T>(this IEnumerable<T> source)
        {
            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext()) return default(T);
                T result = e.Current;
                if (e.MoveNext()) return default(T);
                return result;
            }
        }
        public static T TakeMax<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector)
            where TResult : IComparable
        {
            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext()) return default(T);
                T max = e.Current;
                TResult key = selector(max);
                while (e.MoveNext())
                {
                    TResult key2 = selector(e.Current);
                    if (key2.CompareTo(key) > 0)
                    {
                        key = key2;
                        max = e.Current;
                    }
                }
                return max;
            }
        }
    }
}
