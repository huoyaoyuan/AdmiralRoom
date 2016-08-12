using System.ComponentModel.Composition;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Modules.MasterDataViewer
{
    [Export(typeof(ModuleInfo))]
    [ExportMetadata("Title", "MasterDataViewer")]
    [ExportMetadata("Author", "huoyaoyuan")]
    [ExportMetadata("Description", "MasterDataViewer")]
    [ExportMetadata("ContractVersion", _Version.Version)]
    public class MasterDataViewer : ModuleInfo { }
}
