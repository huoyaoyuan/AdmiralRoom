namespace Huoyaoyuan.AdmiralRoom.Composition
{
    public interface IModuleMetadata
    {
        string Title { get; }
        string Author { get; }
        string Description { get; }
        string ContractVersion { get; }
    }
}
