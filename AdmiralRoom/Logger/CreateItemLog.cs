using System;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public class CreateItemLog
    {
        [Log, Show]
        public DateTime DateTime { get; set; }
        [Log]
        public int SecretaryId { get; set; }
        [Log]
        public int SecretaryLevel { get; set; }
        [Show]
        public string Secretary => $"{Officer.Staff.Current.MasterData.ShipInfo[SecretaryId].Name}(Lv.{SecretaryLevel})";
        [Log, Show]
        public int Item1 { get; set; }
        [Log, Show]
        public int Item2 { get; set; }
        [Log, Show]
        public int Item3 { get; set; }
        [Log, Show]
        public int Item4 { get; set; }
        [Log]
        public bool IsSuccess { get; set; }
        [Show(IsFilter = true)]
        public string Success => IsSuccess ? Properties.Resources.Success : Properties.Resources.Fail;
        [Log]
        public int EquipId { get; set; }
        [Show("EquipmentCreated", IsFilter = true)]
        public string Equipment => Officer.Staff.Current.MasterData.EquipInfo[EquipId].Name;
        [Log, Show]
        public int AdmiralLevel { get; set; }
    }
}
