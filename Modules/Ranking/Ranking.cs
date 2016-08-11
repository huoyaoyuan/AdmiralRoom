using System.Collections.Generic;
using System.ComponentModel.Composition;
using Huoyaoyuan.AdmiralRoom.Composition;

#pragma warning disable CC0021

namespace Huoyaoyuan.AdmiralRoom.Modules.Ranking
{
    [Export(typeof(IModuleInfo))]
    [ExportMetadata("Title", "戦果情報")]
    [ExportMetadata("Author", "huoyaoyuan")]
    [ExportMetadata("Description", "戦果情報")]
    [ExportMetadata("ContractVersion", ContractVersion.Version)]
    public class Ranking : ModuleInfoBase
    {
        public override bool AutoLoadComponents => false;

        public override IList<ISubView> SubViews { get; } = new ISubView[] { new RankingView() };

        public override void Initialize()
        {
            RankingViewModel.Instance = new RankingViewModel();
        }

        public override void Unload()
        {
            RankingViewModel.Instance.Save();
        }
    }
}
