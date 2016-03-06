using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class ShipType : GameObject<api_mst_stype>
    {
        public override int Id => rawdata.api_id;
        public string Name => rawdata.api_name ?? "？？？";
        public string DisplayName => Id == 8 ? "高速戦艦" : Id == 9 ? "低速戦艦" : Name;
        public int SortNo => rawdata.api_sortno;
        public int RepairTimeRatio => rawdata.api_scnt;
        public int BuildShape => rawdata.api_kcnt;
        public ShipType(api_mst_stype api) : base(api) { }
    }
}
