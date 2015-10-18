using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class IDTable<T> : ObservableCollection<T>
        where T : class, IIdentifiable, new()
    {
        public IDTable():base() { }
        public IDTable(List<T> list):base(list) { }
        public IDTable(T[] array) : base(array) { }
        public IDTable(IEnumerable<T> collection) : base(collection) { }
        public new T this[int id]
        {
            get
            {
                foreach(var item in this)
                {
                    if (item.Id == id) return item;
                }
                return default(T);
            }
            set
            {
                if (value.Id != id) throw new ArgumentException();
                for(int i = 0; i< Count; i++)
                {
                    var item = base[i];
                    if(item.Id == id)
                    {
                        base[i] = value;
                        return;
                    }
                    Add(value);
                }
            }
        }

        public void UpdateAll<T2>(T2[] source, Func<T2, int> getid)
        {
            var deletelist = this.ToList();
            foreach(T2 e in source)
            {
                var item = this[getid(e)];
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
    }
}
