using System.ComponentModel.Composition;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Modules.Akashi
{
    [Export(typeof(IModuleInfo))]
    [ExportMetadata("Title", "改修工廠")]
    [ExportMetadata("Author", "huoyaoyuan")]
    [ExportMetadata("Description", "明石の改修工廠")]
    [ExportMetadata("ContractVersion", ContractVersion.Version)]
    public class Akashi : ModuleInfoBase { }
}
