using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Huoyaoyuan.AdmiralRoom.Composition
{
    /// <summary>
    /// Represents a module assembly. Must be unique in the assembly.
    /// Should provide metadata using <see cref="ExportMetadataAttribute"/>:
    /// <para>Title</para>
    /// <para>Author</para>
    /// <para>Description</para>
    /// <para>ContractVersion: should always use <see cref="ContractVersion.Version"/>.</para>
    /// </summary>
    public interface IModuleInfo
    {
        /// <summary>
        /// The components of <see cref="SubViews"/> and <see cref="SubWindows"/> will be filled by the host if true.
        /// Otherwise the inherited class should fill them by self.
        /// </summary>
        bool AutoLoadComponents { get; }
        /// <summary>
        /// The <see cref="ISubView"/>s provided. Should be initialized to not null.
        /// </summary>
        IList<ISubView> SubViews { get; }
        /// <summary>
        /// The <see cref="ISubWindow"/>s provided. Should be initialized to not null.
        /// </summary>
        IList<ISubWindow> SubWindows { get; }
        /// <summary>
        /// Called by the host after <see cref="SubViews"/> and <see cref="SubWindows"/> are filled.
        /// </summary>
        void Initialize();
        /// <summary>
        /// Called by the host when the module will be unloaded, or the application is shuting down.
        /// </summary>
        void Unload();
    }
}
