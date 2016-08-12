using System.ComponentModel.Composition;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Modules.Ranking
{
    [Export(typeof(IModuleInfo))]
    [ExportMetadata("Title", "戦果情報")]
    [ExportMetadata("Author", "huoyaoyuan")]
    [ExportMetadata("Description", "戦果情報")]
    [ExportMetadata("ContractVersion", ContractVersion.Version)]
    public class Ranking : IModuleInfo
    {
        public void Initialize()
        {
            RankingViewModel.Instance = new RankingViewModel();
        }

        public void Unload()
        {
            RankingViewModel.Instance.Save();
        }
    }
}
