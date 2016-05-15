namespace Huoyaoyuan.AdmiralRoom.API
{
    public class map_start_next
    {
        public int api_rashin_flg { get; set; }
        public int api_rashin_id { get; set; }
        public int api_maparea_id { get; set; }
        public int api_mapinfo_no { get; set; }
        public int api_no { get; set; }
        public int api_color_no { get; set; }
        public int api_event_id { get; set; }
        public int api_event_kind { get; set; }
        public int api_next { get; set; }
        public int api_bosscell_no { get; set; }
        public int api_bosscomp { get; set; }
        public int api_comment_kind { get; set; }
        public int api_production_kind { get; set; }
        //public object api_eventmap { get; set; }
        public class itemget
        {
            public int api_usemst { get; set; }
            public int api_id { get; set; }
            public int api_getcount { get; set; }
            public string api_name { get; set; }
            public int api_icon_id { get; set; }
        }
        public itemget[] api_itemget { get; set; }
        public class happening
        {
            public int api_type { get; set; }
            public int api_count { get; set; }
            public int api_usemst { get; set; }
            public int api_mst_id { get; set; }
            public int api_icon_id { get; set; }
            public int api_dentan { get; set; }
            public int api_get_eo_rate { get; set; }
        }
        public happening api_happening { get; set; }
        public itemget api_itemget_eo_comment { get; set; }
        public class select_route
        {
            public int[] api_select_cells { get; set; }
        }
        public select_route api_select_route { get; set; }
        public class airsearch
        {
            public int api_plane_type { get; set; }
            public int api_result { get; set; }
        }
        public airsearch api_airsearch { get; set; }
    }
}
