using System.ComponentModel.Composition;
using Huoyaoyuan.AdmiralRoom;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace RawApiViewer
{
    [Export(typeof(ModuleInfo))]
    [ExportMetadata("Title", "RawApiViewer")]
    [ExportMetadata("Author", "huoyaoyuan")]
    [ExportMetadata("Description", "RawApiViewer")]
    [ExportMetadata("ContractVersion", _Version.Version)]
    public class RawApiViewer : ModuleInfo
    {
        public RawApiViewer()
        {
            RawApiViewModel.Instance = new RawApiViewModel();
        }
    }
}
