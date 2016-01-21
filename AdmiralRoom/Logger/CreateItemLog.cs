using System;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public class CreateItemLog
    {
        [Log]
        public DateTime DateTime { get; set; }
        [Show]
        public string LocalDateTime => DateTime.ToLocalTime().ToString();
        [Log]
        public int SecretaryLevel { get; set; }
        [Log]
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
        [Show(IsFilter = true)]
        public string Success => IsSuccess ? Properties.Resources.Success : Properties.Resources.Fail;
        [Log, Show("EquipmentCreated", IsFilter = true)]
        public string Equipment { get; set; }
        [Log, Show]
        public int AdmiralLevel { get; set; }
    }
}
