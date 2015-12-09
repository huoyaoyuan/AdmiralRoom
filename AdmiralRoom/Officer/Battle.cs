using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Battle
    {
        public class ShipInBattle
        {
            public int Level { get; set; }
            public ShipInfo ShipInfo { get; set; }
            public int MaxHP { get; set; }
            public int FromHP { get; set; }
            public int ToHP { get; set; }
            public int Damage { get; set; }
        }
        public CombinedFleetType FleetType { get; set; }
        public ShipInBattle[] Fleet1 { get; set; }
        public ShipInBattle[] Fleet2 { get; set; }
        public ShipInBattle[] EnemyFleet { get; set; }
        private ShipInBattle this[int index]
        {
            get
            {
                if (index <= 6) return Fleet1[index - 1];
                if (index > 12) return EnemyFleet[index - 13];
                if (Fleet2 != null) return Fleet2[index - 7];
                else return EnemyFleet[index - 7];
            }
        }
        public int[] Formations { get; set; }
        public Battle(sortie_battle api, CombinedFleetType fleettype, BattleManager source)
        {
            FleetType = fleettype;
            Fleet1 = source.SortieFleet1.Ships.Select(x => new ShipInBattle { Level = x.Level, ShipInfo = x.ShipInfo, MaxHP = x.HP.Max, FromHP = x.HP.Current, ToHP = x.HP.Current }).ToArray();
            Fleet2 = source.SortieFleet2?.Ships?.Select(x => new ShipInBattle { Level = x.Level, ShipInfo = x.ShipInfo, MaxHP = x.HP.Max, FromHP = x.HP.Current, ToHP = x.HP.Current }).ToArray();
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
        }
        public void NightBattle(sortie_battle api)
        {
            //TODO
        }
        private static class Delegates
        {
            public static void SetMaxHP(ShipInBattle ship, int hp) => ship.MaxHP = hp;
            public static void SetStartHP(ShipInBattle ship, int hp) => ship.FromHP = ship.ToHP = hp;
            public static void SetDamage(ShipInBattle ship, decimal damage)
            {
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
    //public enum BattleType { Day, Night, Air }
}
