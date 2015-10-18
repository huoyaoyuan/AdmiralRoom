using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huoyaoyuan.AdmiralRoom
{
    static class CollectionHelper
    {
        public static IEnumerable<T2> ArrayOperation<T1,T2>(this IEnumerable<T1> source,Func<T1,T2> func)
        {
            foreach(T1 s in source)
            {
                T2 res = func(s);
                if (res == null) yield break;
                else yield return res;
            }
        }
    }
}
