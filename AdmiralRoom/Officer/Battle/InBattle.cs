using System.Linq;
using Meowtrix.ComponentModel;

namespace Huoyaoyuan.AdmiralRoom.Officer.Battle
{

    public class ShipInBattle : NotificationObject
    {
        public int Index { get; set; }
        public string DisplayName => $"{Index}. {ShipInfo.FullName}";
        public int Level { get; set; }
        public ShipInfo ShipInfo { get; set; }
        public int MaxHP { get; set; }
        public int FromHP { get; set; }
        public int ToHP { get; set; }
        public int Damage { get; set; }
        public int DamageGiven { get; set; }
        public EquipInBattle[] Equipments { get; set; }
        public EquipInBattle EquipmentEx { get; set; }
        public bool IsEnemy { get; set; }
        public EquipInfo DamageControl => new[] { EquipmentEx }.Concat(Equipments).FirstOrDefault(x => x?.EquipInfo.EquipType.Id == 23)?.EquipInfo;
        public bool IsMostDamage { get; set; }
        public bool IsEscaped { get; set; }
        public bool CanAerialTorpedo => Equipments.Any(x => x.EquipInfo.EquipType.Id == 8);
        public bool CanAerialBomb => Equipments.Any(x => x.EquipInfo.EquipType.Id == 7 || x.EquipInfo.EquipType.Id == 11 || x.EquipInfo.EquipType.Id == 57);
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
        public Param TotalFirepower => new Param { Raw = Firepower, Equip = Equipments.Sum(x => x.EquipInfo.FirePower) + EquipmentEx?.EquipInfo.FirePower ?? 0 };
        public Param TotalTorpedo => new Param { Raw = Torpedo, Equip = Equipments.Sum(x => x.EquipInfo.Torpedo) + EquipmentEx?.EquipInfo.Torpedo ?? 0 };
        public Param TotalAA => new Param { Raw = AA, Equip = Equipments.Sum(x => x.EquipInfo.AA) + EquipmentEx?.EquipInfo.AA ?? 0 };
        public Param TotalArmor => new Param { Raw = Armor, Equip = Equipments.Sum(x => x.EquipInfo.Armor) + EquipmentEx?.EquipInfo.Armor ?? 0 };
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
}
