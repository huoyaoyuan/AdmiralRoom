using System.Collections.Generic;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public abstract class Logger<T>
        where T : ILog
    {
        protected readonly PropertyProvider<T> provider;
        protected Logger(bool useshownprovider = false) { provider = new PropertyProvider<T>(useshownprovider); }
        public abstract void Log(T item);
        public abstract void Import(IEnumerable<T> items);
        public abstract IEnumerable<T> Read();
        protected static string GetText(string textkey) => Properties.Resources.ResourceManager.GetString("LogTitle_" + textkey) ?? textkey;
        public string TitleKey { get; set; }
    }
}
