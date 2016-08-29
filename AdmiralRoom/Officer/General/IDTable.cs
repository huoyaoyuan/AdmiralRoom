using System;
using System.Collections.Generic;
using System.Linq;
using Meowtrix;
using Meowtrix.Collections.Generic;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public static class IDTableEx
    {
        public static void UpdateAll<T, T2>(this IDTable<int, T> idtable, IEnumerable<T2> source, Func<T2, int> getid)
            where T : IIdentifiable<int>
        {
            var deletelist = idtable.ToList();
            foreach (T2 e in source)
            {
                T item = idtable[getid(e)];
                if (item != null)
                {
                    ((IUpdatable<T2>)item).Update(e);
                    deletelist.Remove(item);
                }
                else
                    idtable.Add((T)Activator.CreateInstance(typeof(T), e));
            }
            foreach (T item in deletelist)
                idtable.Remove(item);
        }

        public static void UpdateWithoutRemove<T, T2>(this IDTable<int, T> idtable, IEnumerable<T2> source, Func<T2, int> getid)
            where T : IIdentifiable<int>
        {
            foreach (T2 e in source)
            {
                T item = idtable[getid(e)];
                if (item != null)
                {
                    ((IUpdatable<T2>)item).Update(e);
                }
                else
                    idtable.Add((T)Activator.CreateInstance(typeof(T), e));
            }
        }
    }
}
