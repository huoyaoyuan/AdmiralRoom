using System.ComponentModel.Composition;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Modules.MasterDataViewer
{
    [Export(typeof(IModuleInfo))]
    [ExportMetadata("Title", "MasterDataViewer")]
    [ExportMetadata("Author", "huoyaoyuan")]
    [ExportMetadata("Description", "MasterDataViewer")]
    [ExportMetadata("ContractVersion", ContractVersion.Version)]
    public class MasterDataViewer : ModuleInfoBase { }
}
