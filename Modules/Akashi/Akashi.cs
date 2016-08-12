using System.ComponentModel.Composition;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Modules.Akashi
{
    [Export(typeof(ModuleInfo))]
    [ExportMetadata("Title", "改修工廠")]
    [ExportMetadata("Author", "huoyaoyuan")]
    [ExportMetadata("Description", "明石の改修工廠")]
    [ExportMetadata("ContractVersion", _Version.Version)]
    public class Akashi : ModuleInfo { }
}
