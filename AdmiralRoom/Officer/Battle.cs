using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Battle : NotificationObject
    {
        public class ShipInBattle : NotificationObject
        {
            public int Level { get; set; }
            public ShipInfo ShipInfo { get; set; }
            public int MaxHP { get; set; }
            public int FromHP { get; set; }
            public int ToHP { get; set; }
            public int Damage { get; set; }
            public LimitedValue HP => new LimitedValue(ToHP, MaxHP);
            public void EndUpdate() => OnAllPropertyChanged();
        }
        public CombinedFleetType FleetType { get; set; }
        public ShipInBattle[] Fleet1 { get; set; }
        public ShipInBattle[] Fleet2 { get; set; }
        public ShipInBattle[] EnemyFleet { get; set; }
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
        public double FriendDamageRate => (double)Fleet1.Concat(Fleet2 ?? Enumerable.Empty<ShipInBattle>()).Sum(x => x.FromHP - x.ToHP)
            / Fleet1.Concat(Fleet2 ?? Enumerable.Empty<ShipInBattle>()).Sum(x => x.FromHP);
        public double EnemyDamageRate => (double)EnemyFleet.Sum(x => x.FromHP - x.ToHP) / EnemyFleet.Sum(x => x.FromHP);
        public Battle(sortie_battle api, CombinedFleetType fleettype, BattleManager source)
        {
            FleetType = fleettype;
            Fleet1 = source.SortieFleet1.Ships.Select(x => new ShipInBattle { Level = x.Level, ShipInfo = x.ShipInfo, MaxHP = x.HP.Max, FromHP = x.HP.Current, ToHP = x.HP.Current }).ToArray();
            Fleet2 = source.SortieFleet2?.Ships?.Select(x => new ShipInBattle { Level = x.Level, ShipInfo = x.ShipInfo, MaxHP = x.HP.Max, FromHP = x.HP.Current, ToHP = x.HP.Current })?.ToArray();

            if (api.api_formation != null)
            {
                FriendFormation = (Formation)api.api_formation[0];
                EnemyFormation = (Formation)api.api_formation[1];
                Direction = (Direction)api.api_formation[2];
            }

            bool iscombined = fleettype != CombinedFleetType.None;

            EnemyFleet = api.api_ship_ke.Where(x => x != -1).Select(x => new ShipInBattle { ShipInfo = Staff.Current.MasterData.ShipInfo[x] }).ToArray();
            EnemyFleet.ArrayZip(api.api_ship_lv, 1, (x, y) => x.Level = y);

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

            AirBattle(api.api_kouku);
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
        }
        private void AirBattle(sortie_battle.airbattle api)
        {
            if (api == null) return;

            if (api.api_stage3 != null)
            {
                Fleet1.ArrayZip(api.api_stage3.api_fdam, 1, Delegates.SetDamage);
                Fleet2?.ArrayZip(api.api_stage3.api_fdam, 7, Delegates.SetDamage);
                EnemyFleet.ArrayZip(api.api_stage3.api_edam, 1, Delegates.SetDamage);
            }
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
        }
        private void FireAttack(sortie_battle.fire api)
        {
            if (api == null) return;
            api.api_df_list.Zip(api.api_damage, (x, y) => x.Zip(y, (a, b) => Delegates.SetDamage(this[a], b)));
        }
    }
    public enum Formation { 単縦陣 = 1, 複縦陣 = 2, 輪形陣 = 3, 梯形陣 = 4, 単横陣 = 5, 第一警戒航行序列 = 11, 第二警戒航行序列 = 12, 第三警戒航行序列 = 13, 第四警戒航行序列 = 14 }
    public enum Direction { 同航戦 = 1, 反航戦 = 2, T字有利 = 3, T字不利 = 4 }
}
