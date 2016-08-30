using System;
using System.Collections.Generic;
using System.Linq;
using Meowtrix.Collections.Generic;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public static class IDTableEx
    {
        public static void UpdateAll<T, T2>(this IDTable<int, T> idtable, IEnumerable<T2> source, Func<T2, int> getid)
            where T : GameObject<T2>
        {
            var deletelist = idtable.ToList();
            var addlist = new List<T>();
            foreach (T2 e in source)
            {
                T item = idtable[getid(e)];
                if (item != null)
                {
                    item.Update(e);
                    deletelist.Remove(item);
                }
                else
                    addlist.Add((T)Activator.CreateInstance(typeof(T), e));
            }
            if (deletelist.Count > 0)
                idtable.RemoveMany(deletelist);
            if (addlist.Count > 0)
                idtable.AddMany(addlist);
        }

        public static void UpdateWithoutRemove<T, T2>(this IDTable<int, T> idtable, IEnumerable<T2> source, Func<T2, int> getid)
            where T : GameObject<T2>
        {
            var addlist = new List<T>();
            foreach (T2 e in source)
            {
                T item = idtable[getid(e)];
                if (item != null)
                    item.Update(e);
                else
                    addlist.Add((T)Activator.CreateInstance(typeof(T), e));
            }
            if (addlist.Count > 0)
                idtable.AddMany(addlist);
        }
    }
}
