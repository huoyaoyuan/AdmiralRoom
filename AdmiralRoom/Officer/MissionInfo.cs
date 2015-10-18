using System;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class MissionInfo : GameObject<api_mst_mission>, IIdentifiable
    {
        public int Id => rawdata.api_id;
        public int MapArea => rawdata.api_maparea_id;
        public string Name => rawdata.api_name;
        public string Details => rawdata.api_details;
        public TimeSpan Time => TimeSpan.FromMinutes(rawdata.api_time);
        public int Difficulty => rawdata.api_difficulty;
        public decimal UseFuel => rawdata.api_use_fuel;
        public decimal UseBull => rawdata.api_use_bull;
        public int[] WinItem1 => rawdata.api_win_item1;
        public int[] WinItem2 => rawdata.api_win_item2;
        public bool CanCancel => rawdata.api_return_flag != 0;
        public MissionInfo() { }
        public MissionInfo(api_mst_mission api) : base(api) { }
    }
}
