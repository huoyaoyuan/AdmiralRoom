using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace Huoyaoyuan.AdmiralRoom.Composition
{
    internal class ModuleHost
    {
        private ModuleHost() { }
        public static ModuleHost Instance { get; }
        [ImportMany(typeof(IModule))]
        public IList<Lazy<IModule, IDictionary<string, object>>> Modules { get; set; }
        public void Initialize()
        {
            using (var catalog = new AggregateCatalog(
                new AssemblyCatalog(Assembly.GetExecutingAssembly()),
                new DirectoryCatalog("modules")))
            using (var container = new CompositionContainer(catalog))
            {
                container.ComposeParts(this);
            }
        }
    }
}
