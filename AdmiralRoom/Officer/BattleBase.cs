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
            public int DamageGiven { get; set; }
            public EquipInfo[] Equipments { get; set; }
            public EquipInfo DamageControl { get; set; }
            public bool IsMostDamage { get; set; }
            public bool IsEscaped { get; set; }
            public bool CanAerialTorpedo => Equipments.Any(x => x.EquipType.Id == 8);
            public bool CanAerialBomb => Equipments.Any(x => x.EquipType.Id == 7 || x.EquipType.Id == 11);
            public LimitedValue HP => new LimitedValue(ToHP, MaxHP);
            public void EndUpdate() => OnAllPropertyChanged();
            public override string ToString() => $"{ShipInfo.FullName}(Lv.{Level}): " + string.Join(", ", Equipments.Select(x => x.Name));
            public ShipInBattle() { }
            public ShipInBattle(Ship ship)
            {
                Level = ship.Level;
                ShipInfo = ship.ShipInfo;
                MaxHP = ship.HP.Max;
                FromHP = ship.HP.Current;
                ToHP = ship.HP.Current;
                Equipments = ship.Slots.Where(y => y.HasItem).Select(y => y.Item.EquipInfo).ToArray();
                DamageControl = ship.DamageControl;
                IsEscaped = ship.IsEscaped;
                Firepower = ship.Firepower.Current;
                Torpedo = ship.Torpedo.Current;
                AA = ship.AA.Current;
                Armor = ship.Armor.Current;
            }
            public void SetMvp() => IsMostDamage = true;
            public int Firepower { get; set; }
            public int Torpedo { get; set; }
            public int AA { get; set; }
            public int Armor { get; set; }
            public Param TotalFirepower => new Param { Raw = Firepower, Equip = Equipments.Sum(x => x.FirePower) };
            public Param TotalTorpedo => new Param { Raw = Torpedo, Equip = Equipments.Sum(x => x.Torpedo) };
            public Param TotalAA => new Param { Raw = AA, Equip = Equipments.Sum(x => x.AA) };
            public Param TotalArmor => new Param { Raw = Armor, Equip = Equipments.Sum(x => x.Armor) };
            public struct Param
            {
                public int Raw { get; set; }
                public int Equip { get; set; }
                public int Total => Raw + Equip;
                public override string ToString() => $"{Total} ({Raw}+{Equip})";
            }
        }
        public CombinedFleetType FleetType { get; set; }
        public ShipInBattle[] Fleet1 { get; set; }
        public ShipInBattle[] Fleet2 { get; set; }
        public ShipInBattle[] EnemyFleet { get; set; }
        public virtual bool IsBattling => false;

        #region GetShip
        private string _getship;
        public string GetShip
        {
            get { return _getship; }
            set
            {
                if (_getship != value)
                {
                    _getship = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        protected BattleBase() { }
        public BattleBase(BattleManager source)
        {
            Fleet1 = source.SortieFleet1.Ships
                .Select(x => new ShipInBattle(x)).ToArray();
            Fleet2 = source.SortieFleet2?.Ships
                .Select(x => new ShipInBattle(x)).ToArray();
        }
    }
}
