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
        /// Called by the host after loaded.
        /// </summary>
        void Initialize();
        /// <summary>
        /// Called by the host when the module will be unloaded, or the application is shuting down.
        /// </summary>
        void Unload();
    }
}
