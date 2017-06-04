using System.Collections.Generic;
using Meowtrix;

namespace Huoyaoyuan.AdmiralRoom.Officer.General
{
    public class IdentifiableEqualityComparer<T, TKey> : IEqualityComparer<T>
        where T : IIdentifiable<TKey>
    {
        public bool Equals(T x, T y)
        {
            if (x == null)
            {
                if (y == null) return true;
                else return false;
            }
            else
            {
                if (y == null) return false;
                else return EqualityComparer<TKey>.Default.Equals(x.Id, y.Id);
            }
        }
        public int GetHashCode(T obj) => obj?.GetHashCode() ?? 0;
    }
}
