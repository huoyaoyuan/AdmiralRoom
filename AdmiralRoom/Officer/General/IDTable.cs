using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class IDTable<T> : ICollection<T>
        where T : class, IIdentifiable, new()
    {
        private Dictionary<int, T> dict = new Dictionary<int, T>();
        public IDTable():base() { }
        public IDTable(IEnumerable<T> collection) { dict = new Dictionary<int, T>(collection.ToDictionary(x => x.Id)); }
        public void Add(T item) => dict.Add(item.Id, item);
        public bool Remove(T item) => dict.Remove(item.Id);
        IEnumerator IEnumerable.GetEnumerator() => dict.Values.GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => dict.Values.GetEnumerator();
        public void Clear() => dict.Clear();
        public bool Contains(T item) => dict.ContainsValue(item);
        public void CopyTo(T[] array, int index) { throw new NotSupportedException(); }
        public int Count => dict.Count;
        public bool IsReadOnly => false;
        public T this[int index]
        {
            get { return dict[index]; }
            set
            {
                if (value.Id != index) throw new ArgumentException();
                dict[index] = value;
            }
        }
        public void UpdateAll<T2>(IEnumerable<T2> source, Func<T2, int> getid)
        {
            var deletelist = dict.Values.ToList();
            foreach(T2 e in source)
            {
                var item = dict[getid(e)];
                if (item != null)
                {
                    (item as IUpdatable<T2>).Update(e);
                    deletelist.Remove(item);
                }
                else
                {
                    item = new T();
                    (item as IUpdatable<T2>).Update(e);
                    Add(item);
                }
            }
            foreach (T item in deletelist)
            {
                if (item is IDisposable)
                    (item as IDisposable).Dispose();
                Remove(item);
            }
        }

        public void UpdateWithoutRemove<T2>(IEnumerable<T2> source,Func<T2,int> getid)
        {
            foreach (T2 e in source)
            {
                var item = dict[getid(e)];
                if (item != null)
                {
                    (item as IUpdatable<T2>).Update(e);
                }
                else
                {
                    item = new T();
                    (item as IUpdatable<T2>).Update(e);
                    Add(item);
                }
            }
        }
    }
}
