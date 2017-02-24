using System.Collections.Generic;
using System.Linq;
using Meowtrix.ComponentModel;

namespace Huoyaoyuan.AdmiralRoom.Officer.Battle
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
            public EquipInBattle[] Equipments { get; set; }
            public EquipInBattle EquipmentEx { get; set; }
            public EquipInfo DamageControl => new[] { EquipmentEx }.Concat(Equipments).FirstOrDefault(x => x?.EquipInfo.EquipType.Id == 23)?.EquipInfo;
            public bool IsMostDamage { get; set; }
            public bool IsEscaped { get; set; }
            public bool CanAerialTorpedo => Equipments.Any(x => x.EquipInfo.EquipType.Id == 8);
            public bool CanAerialBomb => Equipments.Any(x => x.EquipInfo.EquipType.Id == 7 || x.EquipInfo.EquipType.Id == 11);
            public LimitedValue HP => new LimitedValue(ToHP, MaxHP);
            public void EndUpdate() => OnAllPropertyChanged();
            public override string ToString()
                => $"{ShipInfo.FullName}(Lv.{Level}): " + string.Join<EquipInBattle>(", ", Equipments) + (EquipmentEx != null ? "|" + EquipmentEx : string.Empty);
            public ShipInBattle() { }
            public ShipInBattle(Ship ship)
            {
                Level = ship.Level;
                ShipInfo = ship.ShipInfo;
                MaxHP = ship.HP.Max;
                FromHP = ship.HP.Current;
                ToHP = ship.HP.Current;
                Equipments = ship.Slots.Where(y => y.HasItem).Select(y => new EquipInBattle(y.Item)).ToArray();
                if (ship.SlotEx.HasItem)
                    EquipmentEx = new EquipInBattle(ship.SlotEx.Item);

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
            public Param TotalFirepower => new Param { Raw = Firepower, Equip = Equipments.Sum(x => x.EquipInfo.FirePower) };
            public Param TotalTorpedo => new Param { Raw = Torpedo, Equip = Equipments.Sum(x => x.EquipInfo.Torpedo) };
            public Param TotalAA => new Param { Raw = AA, Equip = Equipments.Sum(x => x.EquipInfo.AA) };
            public Param TotalArmor => new Param { Raw = Armor, Equip = Equipments.Sum(x => x.EquipInfo.Armor) };
            public struct Param
            {
                public int Raw { get; set; }
                public int Equip { get; set; }
                public int Total => Raw + Equip;
                public override string ToString() => $"{Total} ({Raw}+{Equip})";
            }
        }
        public class EquipInBattle
        {
            public EquipInfo EquipInfo { get; set; }
            public int ImproveLevel { get; set; }
            public int AirProficiency { get; set; }
            public override string ToString()
            {
                string s = EquipInfo.Name;
                if (ImproveLevel > 0 && ImproveLevel < 10) s += $" ★+{ImproveLevel}";
                if (ImproveLevel == 10) s += " ★max";
                if (AirProficiency > 0) s += $" +{AirProficiency}";
                return s;
            }
            public EquipInBattle(Equipment equip)
            {
                EquipInfo = equip.EquipInfo;
                ImproveLevel = equip.ImproveLevel;
                AirProficiency = equip.AirProficiency;
            }
            public EquipInBattle(EquipInfo info)
            {
                EquipInfo = info;
            }
        }
        public CombinedFleetType FleetType { get; set; }
        public ShipInBattle[] Fleet1 { get; set; }
        public ShipInBattle[] Fleet2 { get; set; }
        public ShipInBattle[] EnemyFleet { get; set; }
        public ShipInBattle[] EnemyFleet2 { get; set; }
        public int[] EnemyShipIds { get; protected set; }
        public IEnumerable<ShipInBattle> AllFriends => Fleet1.ConcatNotNull(Fleet2);
        public IEnumerable<ShipInBattle> AllEnemies => EnemyFleet.ConcatNotNull(EnemyFleet2);
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
