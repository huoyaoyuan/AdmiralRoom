using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class ShipType : GameObject<api_mst_stype>
    {
        public override int Id => rawdata.api_id;
        public string Name => rawdata.api_name ?? "？？？";
        public int SortNo => rawdata.api_sortno;
        public int RepairTimeRatio => rawdata.api_scnt;
        public int BuildShape => rawdata.api_kcnt;
        public ShipType() { }
        public ShipType(api_mst_stype api) : base(api) { }
    }
}
