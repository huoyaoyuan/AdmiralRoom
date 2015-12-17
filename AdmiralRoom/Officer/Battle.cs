using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;
using static System.Math;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Battle : BattleBase
    {
        public override bool IsBattling => true;
        private ShipInBattle this[int index]
        {
            get
            {
                try
                {
                    if (index <= 6) return Fleet1[index - 1];
                    if (index > 12) return EnemyFleet[index - 13];
                    if (Fleet2 != null) return Fleet2[index - 7];
                    else return EnemyFleet[index - 7];
                }
                catch { return null; }
            }
        }
        public Formation FriendFormation { get; set; }
        public Formation EnemyFormation { get; set; }
        public Direction Direction { get; set; }
        public int AnonymousFriendDamage { get; set; }
        public int AnonymousEnemyDamage { get; set; }
        public class AirCombat
        {
            public AirControl AirControl { get; set; }
            public LimitedValue FriendStage1 { get; set; }
            public LimitedValue EnemyStage1 { get; set; }
            public LimitedValue FriendStage2 { get; set; }
            public LimitedValue EnemyStage2 { get; set; }
        }
        public AirCombat AirCombat1 { get; set; }
        public AirCombat AirCombat2 { get; set; }
        public double FriendDamageRate => (double)Fleet1.ConcatNotNull(Fleet2).Sum(x => x.FromHP - x.ToHP)
            / Fleet1.ConcatNotNull(Fleet2).Sum(x => x.FromHP);
        public double EnemyDamageRate => (double)EnemyFleet.Sum(x => x.FromHP - x.ToHP) / EnemyFleet.Sum(x => x.FromHP);
        public int FriendLostCount => Fleet1.ConcatNotNull(Fleet2).Where(x => x.ToHP <= 0).Count();
        public int EnemySinkCount => EnemyFleet.Where(x => x.ToHP <= 0).Count();
        public WinRank WinRank
        {
            get
            {
                int fl = FriendLostCount;
                int es = EnemySinkCount;
                int fd = (int)(FriendDamageRate * 100);
                int ed = (int)(EnemyDamageRate * 100);
                if (fl == 0)
                {
                    if (es == EnemyFleet.Length)
                    {
                        if (fd <= 0) return WinRank.Perfect;
                        else return WinRank.S;
                    }
                    if (es >= Round(EnemyFleet.Length * 0.6)) return WinRank.A;
                    if (EnemyFleet[0].ToHP <= 0) return WinRank.B;
                    if (ed > fd * 2.5) return WinRank.B;
                    if (ed > fd * 0.9) return WinRank.C;
                    return WinRank.D;
                }
                else
                {
                    if (es == EnemyFleet.Length) return WinRank.B;
                    if (EnemyFleet[0].ToHP <= 0 && fl < es) return WinRank.B;
                    if (ed > fd * 2.5) return WinRank.B;
                    if (ed > fd * 0.9) return WinRank.C;
                    if (fl >= Round(Fleet1.ConcatNotNull(Fleet2).Count() * 0.6)) return WinRank.E;
                    return WinRank.D;
                }
            }
        }
        public Battle(sortie_battle api, CombinedFleetType fleettype, BattleManager source)
        {
            FleetType = fleettype;
            Fleet1 = (source.SortieFleet1?.Ships ?? Staff.Current.Homeport.Fleets[api.api_deck_id + api.api_dock_id].Ships)
                .Select(x => new ShipInBattle(x)).ToArray();
            Fleet2 = source.SortieFleet2?.Ships
                .Select(x => new ShipInBattle(x)).ToArray();

            if (api.api_formation != null)
            {
                FriendFormation = (Formation)api.api_formation[0];
                EnemyFormation = (Formation)api.api_formation[1];
                Direction = (Direction)api.api_formation[2];
            }

            bool iscombined = fleettype != CombinedFleetType.None;

            EnemyFleet = api.api_ship_ke.Where(x => x != -1).Select(x => new ShipInBattle { ShipInfo = Staff.Current.MasterData.ShipInfo[x] }).ToArray();
            EnemyFleet.ArrayZip(api.api_ship_lv, 1, (x, y) => x.Level = y);
            EnemyFleet.ArrayZip(api.api_eSlot, 0, (x, y) => x.Equipments = y.Where(z => z != -1).Select(z => Staff.Current.MasterData.EquipInfo[z]).ToArray());

            Fleet1.ArrayZip(api.api_maxhps, 1, Delegates.SetMaxHP);
            if (iscombined)
            {
                Fleet2.ArrayZip(api.api_maxhps, 7, Delegates.SetMaxHP);
                EnemyFleet.ArrayZip(api.api_maxhps, 13, Delegates.SetMaxHP);
            }
            else EnemyFleet.ArrayZip(api.api_maxhps, 7, Delegates.SetMaxHP);

            Fleet1.ArrayZip(api.api_nowhps, 1, Delegates.SetStartHP);
            if (iscombined)
            {
                Fleet2.ArrayZip(api.api_nowhps, 7, Delegates.SetStartHP);
                EnemyFleet.ArrayZip(api.api_nowhps, 13, Delegates.SetStartHP);
            }
            else EnemyFleet.ArrayZip(api.api_nowhps, 7, Delegates.SetStartHP);

            AirCombat1 = AirBattle(api.api_kouku);
            AirCombat2 = AirBattle(api.api_kouku2);
            SupportAttack(api.api_support_info);
            TorpedoAttack(api.api_opening_atack);
            FireAttack(api.api_hougeki1);
            FireAttack(api.api_hougeki2);
            FireAttack(api.api_hougeki3);
            TorpedoAttack(api.api_raigeki);
            NightBattle(api);
        }
        public void NightBattle(sortie_battle api)
        {
            FireAttack(api.api_hougeki);
            Fleet1.ForEach(x => x.EndUpdate());
            Fleet2?.ForEach(x => x.EndUpdate());
            EnemyFleet.ForEach(x => x.EndUpdate());
            OnAllPropertyChanged();
        }
        private static class Delegates
        {
            public static void SetMaxHP(ShipInBattle ship, int hp) => ship.MaxHP = hp;
            public static void SetStartHP(ShipInBattle ship, int hp) => ship.FromHP = ship.ToHP = hp;
            public static void SetDamage(ShipInBattle ship, decimal damage)
            {
                if (ship == null) return;
                ship.ToHP -= (int)damage;
                if (ship.ToHP <= 0) ship.ToHP = 0;
                ship.Damage += (int)damage;
            }
            public static void SetGiveDamage(ShipInBattle ship, decimal damage)
            {
                if (ship == null) return;
                ship.DamageGiven += (int)damage;
            }
        }
        private AirCombat AirBattle(sortie_battle.airbattle api)
        {
            if (api == null) return null;
            AirCombat combat = new AirCombat();
            if (api.api_stage1 != null)//stage1一直都有吧
            {
                combat.AirControl = (AirControl)api.api_stage1.api_disp_seiku;
                combat.FriendStage1 = new LimitedValue(api.api_stage1.api_f_count - api.api_stage1.api_f_lostcount, api.api_stage1.api_f_count);
                combat.EnemyStage1 = new LimitedValue(api.api_stage1.api_e_count - api.api_stage1.api_e_lostcount, api.api_stage1.api_e_count);
            }
            if (api.api_stage2 != null)
            {
                combat.FriendStage2 = new LimitedValue(api.api_stage2.api_f_count - api.api_stage2.api_f_lostcount, api.api_stage2.api_f_count);
                combat.EnemyStage2 = new LimitedValue(api.api_stage2.api_e_count - api.api_stage2.api_e_lostcount, api.api_stage2.api_e_count);
            }
            if (api.api_stage3 != null)
            {
                Fleet1.ArrayZip(api.api_stage3.api_fdam, 1, Delegates.SetDamage);
                Fleet2?.ArrayZip(api.api_stage3.api_fdam, 7, Delegates.SetDamage);
                EnemyFleet.ArrayZip(api.api_stage3.api_edam, 1, Delegates.SetDamage);
            }
            return combat;
        }
        private void SupportAttack(sortie_battle.support api)
        {
            if (api == null) return;
            AirBattle(api.api_support_airatack);
            if (api.api_support_hourai != null)
                EnemyFleet.ArrayZip(api.api_support_hourai.api_damage, 1, Delegates.SetDamage);
        }
        private void TorpedoAttack(sortie_battle.torpedo api)
        {
            if (api == null) return;
            var torpedofleet = Fleet2 ?? Fleet1;
            torpedofleet.ArrayZip(api.api_fdam, 1, Delegates.SetDamage);
            EnemyFleet.ArrayZip(api.api_edam, 1, Delegates.SetDamage);
            torpedofleet.ArrayZip(api.api_fydam, 1, Delegates.SetGiveDamage);
            EnemyFleet.ArrayZip(api.api_eydam, 1, Delegates.SetGiveDamage);
        }
        private void FireAttack(sortie_battle.fire api)
        {
            if (api == null) return;
            api.api_df_list.Zip(api.api_damage, (x, y) => x.Zip(y, (a, b) => Delegates.SetDamage(this[a], b)));
            api.api_damage.ArrayZip(api.api_at_list, 1, (x, y) => x.ForEach(d => Delegates.SetGiveDamage(this[y], d)));
        }
    }
    public enum Formation { 単縦陣 = 1, 複縦陣 = 2, 輪形陣 = 3, 梯形陣 = 4, 単横陣 = 5, 第一警戒航行序列 = 11, 第二警戒航行序列 = 12, 第三警戒航行序列 = 13, 第四警戒航行序列 = 14 }
    public enum Direction { 同航戦 = 1, 反航戦 = 2, T字有利 = 3, T字不利 = 4 }
    public enum WinRank { Perfect, S, A, B, C, D, E }
    public enum AirControl { 制空互角 = 0, 制空権確保 = 1, 航空優勢 = 2, 航空劣勢 = 3, 制空権喪失 = 4 }
}
