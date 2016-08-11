using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Meowtrix.Linq;

namespace Huoyaoyuan.AdmiralRoom.Composition
{
    internal class ModuleHost
    {
        private ModuleHost() { }
        public static ModuleHost Instance { get; } = new ModuleHost();
        public IList<Lazy<IModuleInfo, IModuleMetadata>> Modules { get; private set; }
        public List<ISubView> SubViews { get; } = new List<ISubView>();
        public List<ISubWindow> SubWindows { get; } = new List<ISubWindow>();
        public void Initialize()
        {
            var a = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            using (var container = new CompositionContainer(a))
            {
                SubViews.AddRange(container.GetExportedValues<ISubView>());
                SubWindows.AddRange(container.GetExportedValues<ISubWindow>());
            }

            try
            {
                var d = new DirectoryCatalog("modules");
                using (var container = new CompositionContainer(d))
                    Modules = container.GetExports<IModuleInfo, IModuleMetadata>().ToList();
                //TODO: verify contract version
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            foreach (var module in Modules)
            {
                if (module.Value.AutoLoadComponents)
                {
                    var c = new AssemblyCatalog(module.Value.GetType().Assembly);
                    using (var container = new CompositionContainer(c))
                    {
                        module.Value.SubViews.AddRange(container.GetExportedValues<ISubView>());
                        module.Value.SubWindows.AddRange(container.GetExportedValues<ISubWindow>());
                    }
                }
                SubViews.AddRange(module.Value.SubViews);
                SubWindows.AddRange(module.Value.SubWindows);
                module.Value.Initialize();
            }
        }
        public void Unload()
        {
            foreach (var module in Modules)
                module.Value.Unload();
        }
    }
}
