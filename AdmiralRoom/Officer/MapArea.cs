using System.Collections.Generic;
using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class MapArea : GameObject<api_mst_maparea>
    {
        public override int Id => rawdata.api_id;
        public string Name => rawdata.api_name;
        public bool IsEventArea => rawdata.api_type != 0;
        public IEnumerable<MapInfo> Maps => Staff.Current.MasterData.MapInfos.Where(x => x.AreaNo == this.Id);
        public MapArea() { }
        public MapArea(api_mst_maparea api) : base(api) { }
    }
}
