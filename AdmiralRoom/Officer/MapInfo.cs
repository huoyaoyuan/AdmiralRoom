using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class MapInfo : GameObject<api_mst_mapinfo>
    {
        public override int Id => rawdata.api_id;
        public int AreaNo => rawdata.api_maparea_id;
        public MapArea Area => Staff.Current.MasterData.MapAreas[AreaNo];
        public int No => rawdata.api_no;
        public string Name => rawdata.api_name;
        public int Level => rawdata.api_level;
        public string OperationName => rawdata.api_opetext;
        public string Info => rawdata.api_infotext.Replace("<br>", "");
        public int[] GetItem => rawdata.api_item;
        public int MapHP => rawdata.api_maphp ?? 0;
        public int RequiredDefeatCount => rawdata.api_required_defeat_count ?? 1;
        public bool IsNormalFleet => rawdata.api_sally_flag[0] == 1;
        public bool CanCombinedCarrier => (rawdata.api_sally_flag[1] & 1) != 0;
        public bool CanCombinedSurface => (rawdata.api_sally_flag[1] & 2) != 0;
        public MapInfo() { }
        public MapInfo(api_mst_mapinfo api) : base(api) { }
    }
}
