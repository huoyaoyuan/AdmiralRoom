namespace Huoyaoyuan.AdmiralRoom.API
{
    public class hokyu_charge
    {
        public hokyu_ship[] api_ship { get; set; }
        public int[] api_material { get; set; }
        public int api_use_bou { get; set; }
    }
    public class hokyu_ship
    {
        public int api_id { get; set; }
        public int api_fuel { get; set; }
        public int api_bull { get; set; }
        public int[] api_onslot { get; set; }
    }
}
