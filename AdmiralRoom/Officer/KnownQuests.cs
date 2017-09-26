using Meowtrix.Collections.Generic;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    partial class QuestManager
    {
        public static IDTable<int, QuestInfo> KnownQuests { get; } = new IDTable<int, QuestInfo>()
        {
            new QuestTarget(Counters.BattleCounter.Instance, 210, QuestPeriod.Daily, 10), //敵艦隊を10回邀撃せよ！
            new QuestTarget(StaticCounters.TransportCounter, 218, QuestPeriod.Daily, 3, 212), //敵補給艦を3隻撃沈せよ！
            new QuestTarget(StaticCounters.CVCounter, 211, QuestPeriod.Daily, 3), //敵空母を3隻撃沈せよ！
            new QuestTarget(StaticCounters.TransportCounter, 212, QuestPeriod.Daily, 5, 218), //敵輸送船団を叩け！
            new QuestTarget(StaticCounters.Map2Counter, 226, QuestPeriod.Daily, 5), //南西諸島海域の制海権を握れ！
            new QuestTarget(StaticCounters.SSCounter, 230, QuestPeriod.Daily, 6), //敵潜水艦を制圧せよ！
            new QuestInfo(new[]
            {
                new QuestTarget(Counters.SortieCounter.Instance, 214, QuestPeriod.Weekly, 36),
                new QuestTarget(StaticCounters.BossCounter, 214, QuestPeriod.Weekly, 24),
                new QuestTarget(StaticCounters.BossWinCounter, 214, QuestPeriod.Weekly, 12),
                new QuestTarget(Counters.BattleSRankCounter.Instance, 214, QuestPeriod.Weekly, 6)
            }, 1), //あ号作戦
            new QuestTarget(StaticCounters.CVCounter, 220, QuestPeriod.Weekly, 20), //い号作戦
            new QuestTarget(StaticCounters.TransportCounter, 213, QuestPeriod.Weekly, 20), //海上通商破壊作戦
            new QuestTarget(StaticCounters.TransportCounter, 221, QuestPeriod.Weekly, 50), //ろ号作戦
            new QuestTarget(StaticCounters.SSCounter, 228, QuestPeriod.Weekly, 15), //海上護衛戦
            new QuestTarget(StaticCounters.Map3Counter, 241, QuestPeriod.Weekly, 5), //敵北方艦隊主力を撃滅せよ！
            new QuestTarget(StaticCounters.Map4Counter, 229, QuestPeriod.Weekly, 12), //敵東方艦隊を撃滅せよ！
            new QuestTarget(StaticCounters.Map1_5Counter, 261, QuestPeriod.Weekly, 3), //海上輸送路の安全確保に努めよ！
            new QuestTarget(StaticCounters.Map1_5Counter, 265, QuestPeriod.Monthly, 10), //海上護衛強化月間
            new QuestTarget(Counters.PracticeCounter.Instance, 303, QuestPeriod.Daily, 3), //「演習」で練度向上！
            new QuestTarget(Counters.PracticeWinCounter.Instance, 304, QuestPeriod.Daily, 5), //「演習」で他提督を圧倒せよ！
            new QuestTarget(Counters.PracticeWinCounter.Instance, 302, QuestPeriod.Weekly, 20), //大規模演習
            new QuestTarget(Counters.PracticeWinCounter.Instance, 311, QuestPeriod.Daily, 7), //精鋭艦隊演習
            new QuestTarget(Counters.RepairCounter.Instance, 503, QuestPeriod.Daily, 5), //艦隊大整備！
            new QuestTarget(Counters.ChargeCounter.Instance, 504, QuestPeriod.Daily, 15), //艦隊酒保祭り！
            new QuestTarget(Counters.ItemDestroyCounter.Instance, 613, QuestPeriod.Weekly, 24), //資源の再利用
            new QuestTarget(Counters.ExpeditionCounter.Instance, 402, QuestPeriod.Daily, 3), //「遠征」を3回成功させよう！
            new QuestTarget(Counters.ExpeditionCounter.Instance, 403, QuestPeriod.Daily, 10), //「遠征」を10回成功させよう！
            new QuestTarget(Counters.ExpeditionCounter.Instance, 404, QuestPeriod.Weekly, 30), //大規模遠征作戦、発令！
            new QuestTarget(Counters.ExpeditionTokyoCounter.Instance, 411, QuestPeriod.Weekly, 6), //南方への鼠輸送を継続実施せよ!
            new QuestTarget(Counters.PowerUpCounter.Instance, 703, QuestPeriod.Weekly, 15), //「近代化改修」を進め、戦備を整えよ！
        };
    }
}
