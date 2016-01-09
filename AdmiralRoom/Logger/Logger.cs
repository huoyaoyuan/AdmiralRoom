using System.Collections.Generic;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public abstract class Logger<T>
    {
        protected Logger() { }
        public abstract void Add(T item);
        public abstract void Import(IEnumerable<T> items);
        public abstract IEnumerable<T> Read();
    }
}
