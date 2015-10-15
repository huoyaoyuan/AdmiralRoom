using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class IDTable<T> : ObservableCollection<T>
        where T : class, IIdentifiable
    {
        public IDTable():base() { }
        public IDTable(List<T> list):base(list) { }
        public IDTable(T[] array) : base(array) { }
        public new T this[int id]
        {
            get
            {
                foreach(var item in this)
                {
                    if (item.Id == id) return item;
                }
                return null;
            }
            set
            {
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
    }
}
