using System.Linq;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class BattleBase : NotificationObject
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
        public virtual bool IsBattling => false;
        protected BattleBase() { }
        public BattleBase(BattleManager source)
        {
            Fleet1 = source.SortieFleet1.Ships
                .Select(x => new ShipInBattle { Level = x.Level, ShipInfo = x.ShipInfo, MaxHP = x.HP.Max, FromHP = x.HP.Current, ToHP = x.HP.Current }).ToArray();
            Fleet2 = source.SortieFleet2?.Ships
                .Select(x => new ShipInBattle { Level = x.Level, ShipInfo = x.ShipInfo, MaxHP = x.HP.Max, FromHP = x.HP.Current, ToHP = x.HP.Current }).ToArray();
        }
    }
}
