namespace Huoyaoyuan.AdmiralRoom.API
{
    public class mission_result
    {
        public int[] api_ship_id { get; set; }
        public int api_clear_result { get; set; }
        public int api_get_exp { get; set; }
        public int api_member_lv { get; set; }
        public int api_member_exp { get; set; }
        public int[] api_get_ship_exp { get; set; }
        public int[][] api_get_exp_lvup { get; set; }
        public string api_maparea_name { get; set; }
        public string api_detail { get; set; }
        public string api_quest_name { get; set; }
        public int api_quest_level { get; set; }
        //public int[] api_get_material { get; set; }//滚你的-1
        public int[] api_useitem_flag { get; set; }
        //public object api_get_item1 { get; set; }
        //public object api_get_item2 { get; set; }
    }
}
