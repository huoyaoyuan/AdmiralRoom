namespace Huoyaoyuan.AdmiralRoom.API
{
    public class getmember_require_info
    {
        public getmember_basic api_basic { get; set; }
        public getmember_slotitem[] api_slot_item { get; set; }
        public getmember_kdock[] api_kdock { get; set; }
    }
}
