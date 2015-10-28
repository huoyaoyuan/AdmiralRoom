namespace Huoyaoyuan.AdmiralRoom.API
{
    public class req_getship
    {
        public int api_id { get; set; }
        public int api_ship_id { get; set; }
        public getmember_kdock[] api_kdock { get; set; }
        public api_ship api_ship { get; set; }
        public getmember_slotitem[] api_slotitem { get; set; }
    }
}
