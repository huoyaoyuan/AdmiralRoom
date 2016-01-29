using System;

#pragma warning disable CC0021

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public class MissionLog : ILog
    {
        [Log]
        public DateTime DateTime { get; set; }
        [Show("DateTime")]
        public string DateTimeLocal => DateTime.ToLocalTime().ToString();
        [Log, Show, Filter]
        public string MissionName { get; set; }
        [Log]
        public int ResultRank { get; set; }
        [Show, Filter]
        public string Result => ResultRank == 0 ? Properties.Resources.Expedition_Fail :
            ResultRank == 1 ? Properties.Resources.Expedition_Success : Properties.Resources.Expedition_Great;
        [Log, Show]
        public int Item1 { get; set; }
        [Log, Show]
        public int Item2 { get; set; }
        [Log, Show]
        public int Item3 { get; set; }
        [Log, Show]
        public int Item4 { get; set; }
        [Log]
        public int UseItem1 { get; set; }
        [Show("UseItem1"), Filter]
        public string UseItem1Name => Officer.Staff.Current.MasterData.UseItems?[UseItem1]?.Name ?? "";
        [Log]
        public int UseItem1Count { get; set; }
        [Show("UseItemCount")]
        public string UseItem1CountString => UseItem1Count > 0 ? UseItem1Count.ToString() : "";
        [Log]
        public int UseItem2 { get; set; }
        [Show("UseItem2"), Filter]
        public string UseItem2Name => Officer.Staff.Current.MasterData.UseItems?[UseItem2]?.Name ?? "";
        [Log]
        public int UseItem2Count { get; set; }
        [Show("UseItemCount")]
        public string UseItem2CountString => UseItem2Count > 0 ? UseItem2Count.ToString() : "";
    }
}
