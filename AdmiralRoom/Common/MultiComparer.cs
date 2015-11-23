using System;
using System.Collections.Generic;

namespace Huoyaoyuan.AdmiralRoom
{
    class MultiComparer<T> : IComparer<T>
    {
        public IEnumerable<Tuple<Func<T, int>, bool>> Selectors { get; set; }
        public int Compare(T x, T y)
        {
            foreach (var tuple in Selectors)
            {
                int result = tuple.Item1(x) - tuple.Item1(y);
                if (result == 0) continue;
                if (tuple.Item2) result = -result;
                return result;
            }
            return 0;
        }
    }
}
