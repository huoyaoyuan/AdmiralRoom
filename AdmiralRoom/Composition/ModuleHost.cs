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
        public static ModuleHost Instance { get; } = new ModuleHost();
        [ImportMany(typeof(IModule))]
        public IList<Lazy<IModule, IDictionary<string, object>>> Modules { get; } = new List<Lazy<IModule, IDictionary<string, object>>>();
        public void Initialize()
        {
            using (var catalog = new AggregateCatalog(
                new AssemblyCatalog(Assembly.GetExecutingAssembly()),
                new DirectoryCatalog("modules")))
            using (var container = new CompositionContainer(catalog))
            {
                container.ComposeParts(this);
            }
#pragma warning disable CC0020
            ResourceService.Current.CultureChanged += culture =>
            {
                foreach (var module in Modules)
                    module.Value.OnCultureChanged(culture);
            };
#pragma warning restore CC0020
        }
    }
}
