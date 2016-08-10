using System;

#pragma warning disable CC0021

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    class CreateShipLog : ILog
    {
        [Log]
        public DateTime DateTime { get; set; }
        [Show("DateTime")]
        public string LocalDateTime => DateTime.ToLocalTime().ToString();
        [Log]
        public int SecretaryLevel { get; set; }
        [Log]
        public int SecretaryId { get; set; }
        [Filter(nameof(SecretaryId))]
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
        [Log, Show]
        public int Item5 { get; set; }
        [Log]
        public bool IsLSC { get; set; }
        [Show("BuildingType"), Filter]
        public string Type => IsLSC ? StringTable.LSC_Yes : StringTable.LSC_No;
        [Log]
        public int ShipId { get; set; }
        [Show("ShipCreated"), Filter(nameof(ShipId))]
        public string Ship => Officer.Staff.Current.MasterData.ShipInfo?[ShipId]?.Name;
        public int ShipTypeId => Officer.Staff.Current.MasterData.ShipInfo?[ShipId]?.ShipType.Id ?? 0;
        [Show, Filter(nameof(ShipTypeId))]
        public string ShipType => Officer.Staff.Current.MasterData.ShipInfo?[ShipId]?.ShipType.Name;
        [Log, Show]
        public int EmptyDocks { get; set; }
        [Log, Show]
        public int AdmiralLevel { get; set; }
    }
}
