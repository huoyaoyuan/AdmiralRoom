using System;
using Huoyaoyuan.AdmiralRoom.Officer.Battle;

#pragma warning disable CC0021

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    class BattleDropLog : ILog
    {
        [Log]
        public DateTime DateTime { get; set; }
        [Show("DateTime")]
        public string LocalDateTime => DateTime.ToLocalTime().ToString();
        [Log]
        public int MapArea { get; set; }
        [Show("MapArea"), Filter(nameof(MapArea))]
        public string MapAreaId => $"{MapArea / 10}-{MapArea % 10}";
        [Log, Show]
        public string MapAreaName { get; set; }
        [Log, Show, Filter]
        public int MapCell { get; set; }
        [Log]
        public bool IsBOSS { get; set; }
        [Show("BOSS")]
        public string BOSSShown => IsBOSS ? StringTable.Compass_BOSS : "";
        [Filter]
        public string BOSS => IsBOSS ? StringTable.Compass_BOSS : "-";
        [Log]
        public WinRank WinRank { get; set; }
        [Show("WinRank"), Filter(nameof(WinRank))]
        public string WinRankString
        {
            get
            {
                switch (WinRank)
                {
                    case WinRank.Perfect:
                        return StringTable.Battle_WinRank_Perfect;
                    case WinRank.S:
                        return StringTable.Battle_WinRank_S;
                    case WinRank.A:
                        return StringTable.Battle_WinRank_A;
                    case WinRank.B:
                        return StringTable.Battle_WinRank_B;
                    case WinRank.C:
                        return StringTable.Battle_WinRank_C;
                    case WinRank.D:
                        return StringTable.Battle_WinRank_D;
                    case WinRank.E:
                        return StringTable.Battle_WinRank_E;
                    default:
                        return "";
                }
            }
        }
        [Log, Show, Filter]
        public string EnemyFleetName { get; set; }
        [Log]
        public int DropShipId { get; set; }
        [Filter(nameof(DropShipId))]
        public string DropShipName => Officer.Staff.Current.MasterData.ShipInfo?[DropShipId]?.Name ?? "";
        [Show("DropShipName")]
        public string DropShipNameShown => DropShipId == -1 ? StringTable.Log_ShipFull : DropShipName;
        [Log]
        public int DropItem { get; set; }
        [Show("DropItem"), Filter(nameof(DropItem))]
        public string DropItemName => Officer.Staff.Current.MasterData.UseItems?[DropItem]?.Name ?? "";
    }
}
