using System;

#pragma warning disable CC0021

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public class BattleDropLog
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
        public string BOSSShown => IsBOSS ? Properties.Resources.Compass_BOSS : "";
        [Filter]
        public string BOSS => IsBOSS ? Properties.Resources.Compass_BOSS : "-";
        [Log]
        public Officer.WinRank WinRank { get; set; }
        [Show("WinRank"), Filter(nameof(WinRank))]
        public string WinRankString
        {
            get
            {
                switch (WinRank)
                {
                    case Officer.WinRank.Perfect:
                        return Properties.Resources.Battle_WinRank_Perfect;
                    case Officer.WinRank.S:
                        return Properties.Resources.Battle_WinRank_S;
                    case Officer.WinRank.A:
                        return Properties.Resources.Battle_WinRank_A;
                    case Officer.WinRank.B:
                        return Properties.Resources.Battle_WinRank_B;
                    case Officer.WinRank.C:
                        return Properties.Resources.Battle_WinRank_C;
                    case Officer.WinRank.D:
                        return Properties.Resources.Battle_WinRank_D;
                    case Officer.WinRank.E:
                        return Properties.Resources.Battle_WinRank_E;
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
        public string DropShipNameShown => DropShipId == -1 ? Properties.Resources.Log_ShipFull : DropShipName;
        [Log]
        public int DropItem { get; set; }
        [Show("DropItem"), Filter(nameof(DropItem))]
        public string DropItemName => Officer.Staff.Current.MasterData.UseItems?[DropItem]?.Name ?? "";
    }
}
