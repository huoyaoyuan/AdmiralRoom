using System.Collections.Generic;

namespace Huoyaoyuan.AdmiralRoom.Composition
{
    /// <summary>
    /// A base class for <see cref="IModuleInfo"/>.
    /// </summary>
    public abstract class ModuleInfoBase : IModuleInfo
    {
        /// <summary>
        /// The components of <see cref="SubViews"/> and <see cref="SubWindows"/> will be filled by the host.
        /// </summary>
        public virtual bool AutoLoadComponents => true;
        /// <summary>
        /// The <see cref="ISubView"/>s provided. Initialized to an empty <see cref="List{T}"/>
        /// </summary>
        public virtual IList<ISubView> SubViews { get; } = new List<ISubView>();
        /// <summary>
        /// The <see cref="ISubWindow"/>s provided. Initialized to an empty <see cref="List{T}"/>
        /// </summary>
        public virtual IList<ISubWindow> SubWindows { get; } = new List<ISubWindow>();
        /// <summary>
        /// An empty implementation. Should not be called in derived class.
        /// </summary>
        public virtual void Initialize() { }
        /// <summary>
        /// An empty implementation. Should not be called in derived class.
        /// </summary>
        public virtual void Unload() { }
    }
}
