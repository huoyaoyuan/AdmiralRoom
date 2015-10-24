namespace Huoyaoyuan.AdmiralRoom.API
{
    public class getmembet_mapinfo
    {
        public int api_id { get; set; }
        public int api_cleared { get; set; }
        public class eventmap
        {
            public int api_now_maphp { get; set; }
            public int api_max_maphp { get; set; }
            public int api_state { get; set; }
            public int api_selected_rank { get; set; }
        }
        public eventmap api_eventmap { get; set; }
        public int api_exboss_flag { get; set; }
        public int api_defeated_count { get; set; }
    }
}
