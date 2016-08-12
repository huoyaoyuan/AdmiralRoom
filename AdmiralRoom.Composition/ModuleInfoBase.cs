namespace Huoyaoyuan.AdmiralRoom.Composition
{
    /// <summary>
    /// A base class for <see cref="IModuleInfo"/>.
    /// </summary>
    public abstract class ModuleInfoBase : IModuleInfo
    {
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
