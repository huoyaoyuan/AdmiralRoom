using System.Collections.Generic;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public abstract class Logger<T>
    {
        protected readonly PropertyProvider<T> provider = new PropertyProvider<T>();
        protected Logger() { }
        public abstract void Log(T item);
        public abstract void Import(IEnumerable<T> items);
        public abstract IEnumerable<T> Read();
        protected string GetText(string textkey) => Properties.Resources.ResourceManager.GetString("LogTitle_" + textkey) ?? textkey;
    }
}
