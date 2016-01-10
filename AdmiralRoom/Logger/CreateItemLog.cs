using System;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public class CreateItemLog
    {
        [Log]
        public DateTime DateTime { get; set; }
        [Log]
        public int SecretryId { get; set; }
        [Log]
        public int SecretryLevel { get; set; }
        [Log]
        public int Item1 { get; set; }
        [Log]
        public int Item2 { get; set; }
        [Log]
        public int Item3 { get; set; }
        [Log]
        public int Item4 { get; set; }
        [Log]
        public bool IsSuccess { get; set; }
        [Log]
        public int EquipId { get; set; }
        [Log]
        public int AdmiralLevel { get; set; }
    }
}
