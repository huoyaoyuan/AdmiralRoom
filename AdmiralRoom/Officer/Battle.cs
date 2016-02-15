using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;
using static System.Math;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Battle : BattleBase
    {
        public override bool IsBattling => true;
        public MapNodeType BattleType { get; set; }
        private ShipInBattle[] NightOrTorpedo => Fleet2 ?? Fleet1;
        private ShipInBattle FindShip(int index, ShipInBattle[] friend)
        {
            try
            {
                if (index <= 6) return friend[index - 1];
                return EnemyFleet[index - 7];
            }
            catch { return null; }
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
        public int FriendLostCount => Fleet1.ConcatNotNull(Fleet2).Count(x => x.ToHP <= 0);
        public int EnemySinkCount => EnemyFleet.Count(x => x.ToHP <= 0);
        public WinRank WinRank
        {
            get
            {
                int fl = FriendLostCount;
                int es = EnemySinkCount;
                int fd = (int)(FriendDamageRate * 100);
                int ed = (int)(EnemyDamageRate * 100);
                if (BattleType == MapNodeType.AirDefence)//空袭战
                {
                    if (FriendDamageRate <= 0) return WinRank.Perfect;
                    if (fd <= 10) return WinRank.A;
                    if (fd <= 20) return WinRank.B;
                    if (fd <= 50) return WinRank.C;
                    return WinRank.D;
                }
                if (fl == 0)
                {
                    if (es == EnemyFleet.Length)
                    {
                        return FriendDamageRate <= 0 ? WinRank.Perfect : WinRank.S;
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
        public Battle(sortie_battle api, CombinedFleetType fleettype, MapNodeType battletype, BattleManager source)
        {
            FleetType = fleettype;
            BattleType = battletype;
            Fleet1 = (source.SortieFleet1?.Ships ?? Staff.Current.Homeport.Fleets[api.api_deck_id + api.api_dock_id].Ships)
                .Select(x => new ShipInBattle(x)).ToArray();
            Fleet2 = source.SortieFleet2?.Ships
                .Select(x => new ShipInBattle(x)).ToArray();
            if (source.SortieFleet1 == null)//演习
                Staff.Current.Homeport.Fleets[api.api_deck_id + api.api_dock_id].Ships.ForEach(x => x.IgnoreNextCondition());

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
            Fleet2?.ArrayZip(api.api_maxhps_combined, 1, Delegates.SetMaxHP);
            EnemyFleet.ArrayZip(api.api_maxhps, 7, Delegates.SetMaxHP);

            Fleet1.ArrayZip(api.api_nowhps, 1, Delegates.SetStartHP);
            Fleet2?.ArrayZip(api.api_nowhps_combined, 1, Delegates.SetStartHP);
            EnemyFleet.ArrayZip(api.api_nowhps, 7, Delegates.SetStartHP);

            api.api_escape_idx?.ForEach(x => Fleet1[x - 1].IsEscaped = true);
            api.api_escape_idx_combined?.ForEach(x => Fleet2[x - 1].IsEscaped = true);

            AirCombat1 = AirBattle(api.api_kouku, false);
            AirCombat2 = AirBattle(api.api_kouku2, false);
            SupportAttack(api.api_support_info);
            TorpedoAttack(api.api_opening_atack);
            switch (fleettype)
            {
                case CombinedFleetType.None:
                    FireAttack(api.api_hougeki1, Fleet1);
                    FireAttack(api.api_hougeki2, Fleet1);
                    break;
                case CombinedFleetType.Carrier:
                case CombinedFleetType.Transport:
                    FireAttack(api.api_hougeki1, Fleet2);
                    FireAttack(api.api_hougeki2, Fleet1);
                    FireAttack(api.api_hougeki3, Fleet1);
                    break;
                case CombinedFleetType.Surface:
                    FireAttack(api.api_hougeki1, Fleet1);
                    FireAttack(api.api_hougeki2, Fleet1);
                    FireAttack(api.api_hougeki3, Fleet2);
                    break;
            }
            TorpedoAttack(api.api_raigeki);
            NightBattle(api);
        }
        public void NightBattle(sortie_battle api)
        {
            FireAttack(api.api_hougeki, NightOrTorpedo);
            EndApplyBattle();
        }
        private void EndApplyBattle()
        {
            Fleet1.ForEach(Delegates.OnEndUpdate);
            Fleet2?.ForEach(Delegates.OnEndUpdate);
            EnemyFleet.ForEach(Delegates.OnEndUpdate);
            //mvp
            Fleet1.TakeMax(x => x.DamageGiven).SetMvp();
            Fleet2?.TakeMax(x => x.DamageGiven).SetMvp();
            EnemyFleet.TakeMax(x => x.DamageGiven).SetMvp();

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
            public static void OnEndUpdate(ShipInBattle ship)
            {
                ship.EndUpdate();
                ship.IsMostDamage = false;
            }
        }
        private AirCombat AirBattle(sortie_battle.airbattle api, bool issupport)
        {
            if (api == null) return null;
            AirCombat combat = new AirCombat();
            ShipInBattle friendtorpedo = null, friendbomb = null, enemytorpedo = null, enemybomb = null;
            if (!issupport)
            {
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
                friendtorpedo = Fleet1.Where(x => x.CanAerialTorpedo).TakeSingle();
                friendbomb = Fleet1.Where(x => x.CanAerialBomb).TakeSingle();
                enemytorpedo = EnemyFleet.Where(x => x.CanAerialTorpedo).TakeSingle();
                enemybomb = EnemyFleet.Where(x => x.CanAerialBomb).TakeSingle();
            }
            if (api.api_stage3 != null)
            {
                if (!issupport) Fleet1.ArrayZip(api.api_stage3.api_fdam, 1, Delegates.SetDamage);
                EnemyFleet.ArrayZip(api.api_stage3.api_edam, 1, Delegates.SetDamage);
                if (!issupport)
                {
                    for (int i = 1; i < api.api_stage3.api_fdam.Length; i++)
                        if (api.api_stage3.api_frai_flag[i] != 0)
                            if (api.api_stage3.api_fbak_flag[i] != 0)
                                if (enemytorpedo == enemybomb && enemytorpedo != null) enemytorpedo.DamageGiven += (int)api.api_stage3.api_fdam[i];
                                else AnonymousEnemyDamage += (int)api.api_stage3.api_fdam[i];
                            else
                                if (enemytorpedo != null) enemytorpedo.DamageGiven += (int)api.api_stage3.api_fdam[i];
                            else AnonymousEnemyDamage += (int)api.api_stage3.api_fdam[i];
                        else if (api.api_stage3.api_fbak_flag[i] != 0)
                            if (enemybomb != null) enemybomb.DamageGiven += (int)api.api_stage3.api_fdam[i];
                            else AnonymousEnemyDamage += (int)api.api_stage3.api_fdam[i];
                    for (int i = 1; i < api.api_stage3.api_edam.Length; i++)
                        if (api.api_stage3.api_erai_flag[i] != 0)
                            if (api.api_stage3.api_ebak_flag[i] != 0)
                                if (friendtorpedo == friendbomb && friendtorpedo != null) friendtorpedo.DamageGiven += (int)api.api_stage3.api_edam[i];
                                else AnonymousFriendDamage += (int)api.api_stage3.api_edam[i];
                            else
                                if (friendtorpedo != null) friendtorpedo.DamageGiven += (int)api.api_stage3.api_edam[i];
                            else AnonymousFriendDamage += (int)api.api_stage3.api_edam[i];
                        else if (api.api_stage3.api_ebak_flag[i] != 0)
                            if (friendbomb != null) friendbomb.DamageGiven += (int)api.api_stage3.api_edam[i];
                            else AnonymousFriendDamage += (int)api.api_stage3.api_edam[i];
                }
            }
            if (api.api_stage3_combined != null)
            {
                Fleet2.ArrayZip(api.api_stage3_combined.api_fdam, 1, Delegates.SetDamage);
                for (int i = 1; i < api.api_stage3_combined.api_fdam.Length; i++)
                    if (api.api_stage3_combined.api_frai_flag[i] != 0)
                        if (api.api_stage3_combined.api_fbak_flag[i] != 0)
                            if (enemytorpedo == enemybomb && enemytorpedo != null) enemytorpedo.DamageGiven += (int)api.api_stage3_combined.api_fdam[i];
                            else AnonymousEnemyDamage += (int)api.api_stage3_combined.api_fdam[i];
                        else
                            if (enemytorpedo != null) enemytorpedo.DamageGiven += (int)api.api_stage3_combined.api_fdam[i];
                        else AnonymousEnemyDamage += (int)api.api_stage3_combined.api_fdam[i];
                    else if (api.api_stage3_combined.api_fbak_flag[i] != 0)
                        if (enemybomb != null) enemybomb.DamageGiven += (int)api.api_stage3_combined.api_fdam[i];
                        else AnonymousEnemyDamage += (int)api.api_stage3_combined.api_fdam[i];
            }
            return combat;
        }
        private void SupportAttack(sortie_battle.support api)
        {
            if (api == null) return;
            AirBattle(api.api_support_airatack, true);
            if (api.api_support_hourai != null)
                EnemyFleet.ArrayZip(api.api_support_hourai.api_damage, 1, Delegates.SetDamage);
        }
        private void TorpedoAttack(sortie_battle.torpedo api)
        {
            if (api == null) return;
            NightOrTorpedo.ArrayZip(api.api_fdam, 1, Delegates.SetDamage);
            EnemyFleet.ArrayZip(api.api_edam, 1, Delegates.SetDamage);
            NightOrTorpedo.ArrayZip(api.api_fydam, 1, Delegates.SetGiveDamage);
            EnemyFleet.ArrayZip(api.api_eydam, 1, Delegates.SetGiveDamage);
        }
        private void FireAttack(sortie_battle.fire api, ShipInBattle[] fleet)
        {
            if (api == null) return;
            api.api_df_list.ZipEach(api.api_damage, (x, y) => x.ZipEach(y, (a, b) => Delegates.SetDamage(FindShip(a, fleet), b)));
            api.api_damage.ArrayZip(api.api_at_list, 1, (x, y) => x.ForEach(d => Delegates.SetGiveDamage(FindShip(y, fleet), d)));
        }
    }
    public enum Formation { 単縦陣 = 1, 複縦陣 = 2, 輪形陣 = 3, 梯形陣 = 4, 単横陣 = 5, 第一警戒航行序列 = 11, 第二警戒航行序列 = 12, 第三警戒航行序列 = 13, 第四警戒航行序列 = 14 }
    public enum Direction { 同航戦 = 1, 反航戦 = 2, T字有利 = 3, T字不利 = 4 }
    public enum WinRank { Perfect, S, A, B, C, D, E }
    public enum AirControl { 制空互角 = 0, 制空権確保 = 1, 航空優勢 = 2, 航空劣勢 = 3, 制空権喪失 = 4 }
}
