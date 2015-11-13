using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class IDTable<T> : ICollection<T>, INotifyCollectionChanged
        where T : class, IIdentifiable, new()
    {
        private Dictionary<int, T> dict = new Dictionary<int, T>();
        public IDTable() : base() { }
        public IDTable(IEnumerable<T> collection) { dict = new Dictionary<int, T>(collection.ToDictionary(x => x.Id)); }
        public void Add(T item)
        {
            dict.Add(item.Id, item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }
        public bool Remove(T item)
        {
            (item as IDisposable)?.Dispose();
            bool r = dict.Remove(item.Id);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return r;
        }
        public bool Remove(int id) => Remove(dict[id]);
        IEnumerator IEnumerable.GetEnumerator() => dict.Values.GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => dict.Values.GetEnumerator();
        public void Clear()
        {
            dict.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public bool Contains(T item) => dict.ContainsValue(item);
        public void CopyTo(T[] array, int index) { throw new NotSupportedException(); }
        public int Count => dict.Count;
        public bool IsReadOnly => false;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public T this[int index]
        {
            get
            {
                T item;
                dict.TryGetValue(index, out item);
                return item;
            }
            set
            {
                if (value.Id != index) throw new ArgumentException();
                var olditem = dict[index];
                if (dict.ContainsKey(index))
                {
                    dict[index] = value;
                    int rawindex = dict.Values.ToList().IndexOf(value);
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, olditem, rawindex));
                }
                else Add(value);
            }
        }
        public void UpdateAll<T2>(IEnumerable<T2> source, Func<T2, int> getid)
        {
            var deletelist = dict.Values.ToList();
            foreach (T2 e in source)
            {
                T item;
                if (dict.TryGetValue(getid(e), out item))
                {
                    ((IUpdatable<T2>)item).Update(e);
                    deletelist.Remove(item);
                }
                else
                {
                    item = new T();
                    ((IUpdatable<T2>)item).Update(e);
                    Add(item);
                }
            }
            foreach (T item in deletelist)
                Remove(item);
        }

        public void UpdateWithoutRemove<T2>(IEnumerable<T2> source, Func<T2, int> getid)
        {
            foreach (T2 e in source)
            {
                T item;
                if (dict.TryGetValue(getid(e), out item))
                {
                    ((IUpdatable<T2>)item).Update(e);
                }
                else
                {
                    item = new T();
                    ((IUpdatable<T2>)item).Update(e);
                    Add(item);
                }
            }
        }
    }
}
