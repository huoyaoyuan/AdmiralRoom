using System;
using Huoyaoyuan.AdmiralRoom.Officer;
using Meowtrix;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    interface IBattleDetail : IIdentifiable<DateTime>
    {
        DateTime GetTimeStamp();
        BattleDetailViewModel ToViewModel(BattleDropLog log);
        bool IsValid { get; }
    }

    class BattleDetailViewModel
    {
        public BattleDropLog Log { get; set; }
        public MapNode Node { get; set; }
        public object Battle { get; set; }
        public DateTimeOffset Time { get; set; }
        public string LocalTimeString => Time.LocalDateTime.ToString();
    }
}
