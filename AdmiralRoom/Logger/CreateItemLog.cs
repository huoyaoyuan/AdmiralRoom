using System;

#pragma warning disable CC0021

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public class CreateItemLog
    {
        [Log]
        public DateTime DateTime { get; set; }
        [Show("DateTime")]
        public string LocalDateTime => DateTime.ToLocalTime().ToString();
        [Log]
        public int SecretaryLevel { get; set; }
        [Log]
        public int SecretaryId { get; set; }
        [Filter]
        public string SecretaryName => Officer.Staff.Current.MasterData.ShipInfo?[SecretaryId].Name;
        [Show]
        public string Secretary => $"{SecretaryName}(Lv.{SecretaryLevel})";
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
        [Show, Filter]
        public string Success => IsSuccess ? Properties.Resources.Success : Properties.Resources.Fail;
        [Log]
        public int EquipId { get; set; }
        [Show("EquipmentCreated"), Filter]
        public string Equipment => Officer.Staff.Current.MasterData.EquipInfo?[EquipId].Name;
        [Show("EquipmentType"), Filter]
        public string EquipType => Officer.Staff.Current.MasterData.EquipInfo?[EquipId].EquipType.Name;
        [Log, Show]
        public int AdmiralLevel { get; set; }
    }
}
