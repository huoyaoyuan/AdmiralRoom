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
        [Log, Filter]
        public string SecretaryName { get; set; }
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
        [Log, Show("EquipmentCreated"), Filter]
        public string Equipment { get; set; }
        [Log, Show]
        public int AdmiralLevel { get; set; }
    }
}
