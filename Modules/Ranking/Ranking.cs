using System;
using System.ComponentModel.Composition;
using Huoyaoyuan.AdmiralRoom.Composition;

namespace Huoyaoyuan.AdmiralRoom.Modules.Ranking
{
    [Export(typeof(ModuleInfo))]
    [ExportMetadata("Title", "戦果情報")]
    [ExportMetadata("Author", "huoyaoyuan")]
    [ExportMetadata("Description", "戦果情報")]
    [ExportMetadata("ContractVersion", _Version.Version)]
    public class Ranking : ModuleInfo, IDisposable
    {
        public Ranking()
        {
            RankingViewModel.Instance = new RankingViewModel();
        }

        public void Dispose()
        {
            RankingViewModel.Instance.Save();
        }
    }
}
