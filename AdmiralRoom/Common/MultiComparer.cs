using System;
using System.Collections.Generic;

namespace Huoyaoyuan.AdmiralRoom
{
    class MultiComparer<T> : IComparer<T>
    {
        public IEnumerable<(Func<T, int> func, bool reverse)> Selectors { get; set; }
        public int Compare(T x, T y)
        {
            foreach (var selector in Selectors)
            {
                int result = selector.func(x) - selector.func(y);
                if (result == 0) continue;
                if (selector.reverse) result = -result;
                return result;
            }
            return 0;
        }
    }
}
