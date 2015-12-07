using System.Collections.Generic;
using System.Linq;

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
        }
        public CombinedFleetType FleetType { get; set; }
        public IEnumerable<ShipInBattle> Fleet1 { get; set; }
        public IEnumerable<ShipInBattle> Fleet2 { get; set; }
        public IEnumerable<ShipInBattle> EnemyFleet { get; set; }
        public int[] Formations { get; set; }
        public Battle() { }
        public Battle(dynamic api, CombinedFleetType fleettype, BattleManager source, BattleType battletype)
        {
            FleetType = fleettype;
            Fleet1 = source.SortieFleet1.Ships.Select(x => new ShipInBattle { Level = x.Level, ShipInfo = x.ShipInfo, MaxHP = x.HP.Max, FromHP = x.HP.Current, ToHP = x.HP.Current }).ToArray();
            Fleet2 = source.SortieFleet2?.Ships?.Select(x => new ShipInBattle { Level = x.Level, ShipInfo = x.ShipInfo, MaxHP = x.HP.Max, FromHP = x.HP.Current, ToHP = x.HP.Current }).ToArray();
            //EnemyFleet = (api.api_ship_ke as double[]).Skip(1).Select(x => new ShipInBattle { ShipInfo = Staff.Current.MasterData.ShipInfo[(int)x] }).ToArray();
            //TODO:fix
        }
        public void NightBattle(dynamic api)
        {
            //TODO
        }
    }
    public enum BattleType { Day, Night, Air }
}
