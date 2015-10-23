using static Huoyaoyuan.AdmiralRoom.Officer.QuestManager.KnownQuestTargets;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    partial class QuestManager
    {
        public static class KnownQuestTargets
        {
            public static QuestTarget Daily10 = new QuestTarget(Counters.BattleCounter.Instance, 210, QuestPeriod.Daily, 10);
            public static QuestTarget Trans3 = new QuestTarget(StaticCounters.TransportCounter, 218, QuestPeriod.Daily, 3, Trans5);
            public static QuestTarget CV3 = new QuestTarget(StaticCounters.CVCounter, 211, QuestPeriod.Daily, 3);
            public static QuestTarget Trans5 = new QuestTarget(StaticCounters.TransportCounter, 212, QuestPeriod.Daily, 5, Trans3);
            public static QuestTarget Map2 = new QuestTarget(StaticCounters.Map2Counter, 226, QuestPeriod.Daily, 5);
            public static QuestTarget SS6 = new QuestTarget(StaticCounters.SSCounter, 230, QuestPeriod.Daily, 6);
            public static QuestTarget MissionAPart1 = new QuestTarget(Counters.SortieCounter.Instance, 214, QuestPeriod.Weekly, 36);
            public static QuestTarget MissionAPart2 = new QuestTarget(StaticCounters.BossCounter, 214, QuestPeriod.Weekly, 24);
            public static QuestTarget MissionAPart3 = new QuestTarget(StaticCounters.BossWinCounter, 214, QuestPeriod.Weekly, 12);
            public static QuestTarget MissionAPart4 = new QuestTarget(StaticCounters.SRankCounter, 214, QuestPeriod.Weekly, 6);
            public static QuestTarget MissionI = new QuestTarget(StaticCounters.CVCounter, 220, QuestPeriod.Weekly, 20);
            public static QuestTarget Trans20 = new QuestTarget(StaticCounters.TransportCounter, 213, QuestPeriod.Weekly, 20);
            public static QuestTarget MissionRo = new QuestTarget(StaticCounters.TransportCounter, 221, QuestPeriod.Weekly, 50);
            public static QuestTarget SS15 = new QuestTarget(StaticCounters.SSCounter, 228, QuestPeriod.Weekly, 15);
            public static QuestTarget Map3 = new QuestTarget(StaticCounters.Map3Counter, 241, QuestPeriod.Weekly, 5);
            public static QuestTarget Map4 = new QuestTarget(StaticCounters.Map4Counter, 229, QuestPeriod.Weekly, 12);
            public static QuestTarget Map1_5W = new QuestTarget(StaticCounters.Map1_5Counter, 261, QuestPeriod.Weekly, 3);
            public static QuestTarget Map1_5M = new QuestTarget(StaticCounters.Map1_5Counter, 265, QuestPeriod.Monthly, 10);
            public static QuestTarget Practice3 = new QuestTarget(Counters.PracticeCounter.Instance, 303, QuestPeriod.Daily, 3);
            public static QuestTarget Practice5 = new QuestTarget(Counters.PracticeWinCounter.Instance, 304, QuestPeriod.Daily, 5, Practice7);
            public static QuestTarget Practice20 = new QuestTarget(Counters.PracticeWinCounter.Instance, 302, QuestPeriod.Weekly, 20);
            public static QuestTarget Practice7 = new QuestTarget(Counters.PracticeWinCounter.Instance, 311, QuestPeriod.Daily, 7, Practice5);
            public static QuestTarget Repair = new QuestTarget(Counters.RepairCounter.Instance, 503, QuestPeriod.Daily, 5);
            public static QuestTarget Charge = new QuestTarget(Counters.ChargeCounter.Instance, 504, QuestPeriod.Daily, 15);
            public static QuestTarget Destroy = new QuestTarget(Counters.ItemDestroyCounter.Instance, 613, QuestPeriod.Weekly, 24);
            public static QuestTarget Expedition3 = new QuestTarget(Counters.ExpeditionCounter.Instance, 402, QuestPeriod.Daily, 3);
            public static QuestTarget Expedition10 = new QuestTarget(Counters.ExpeditionCounter.Instance, 403, QuestPeriod.Daily, 10);
            public static QuestTarget Expedition30 = new QuestTarget(Counters.ExpeditionCounter.Instance, 404, QuestPeriod.Weekly, 30);
            public static QuestTarget ExpeditionTokyo = new QuestTarget(Counters.ExpeditionTokyoCounter.Instance, 411, QuestPeriod.Weekly, 6);
        }
        public static class KnownQuests
        {
            public static IDTable<QuestInfo> Known = new IDTable<QuestInfo>();
            static KnownQuests()
            {
                Known.Add(new QuestInfo(Daily10));
                Known.Add(new QuestInfo(Trans3));
                Known.Add(new QuestInfo(CV3));
                Known.Add(new QuestInfo(Trans5));
                Known.Add(new QuestInfo(Map2));
                Known.Add(new QuestInfo(SS6));
                Known.Add(new QuestInfo(new[] {
                    MissionAPart1,
                    MissionAPart2,
                    MissionAPart3,
                    MissionAPart4
                }, 1));
                Known.Add(new QuestInfo(MissionI));
                Known.Add(new QuestInfo(Trans20));
                Known.Add(new QuestInfo(MissionRo));
                Known.Add(new QuestInfo(SS15));
                Known.Add(new QuestInfo(Map3));
                Known.Add(new QuestInfo(Map4));
                Known.Add(new QuestInfo(Map1_5M));
                Known.Add(new QuestInfo(Map1_5W));
                Known.Add(new QuestInfo(Practice3));
                Known.Add(new QuestInfo(Practice5));
                Known.Add(new QuestInfo(Practice7));
                Known.Add(new QuestInfo(Practice20));
                Known.Add(new QuestInfo(Repair));
                Known.Add(new QuestInfo(Charge));
                Known.Add(new QuestInfo(Destroy));
                Known.Add(new QuestInfo(Expedition3));
                Known.Add(new QuestInfo(Expedition10));
                Known.Add(new QuestInfo(Expedition30));
                Known.Add(new QuestInfo(ExpeditionTokyo));
            }
        }
    }
}
