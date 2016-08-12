using System;
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
    public abstract class ModuleInfo
    {
        /// <summary>
        /// Get version of the definition assembly of derived class.
        /// </summary>
        /// <returns></returns>
        public virtual Version Version => this.GetType().Assembly.GetName().Version;
    }
}
