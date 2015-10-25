namespace Huoyaoyuan.AdmiralRoom.API
{
    public class sortie_battleresult
    {
        public int[] api_ship_id { get; set; }
        public string api_win_rank { get; set; }
        public int api_get_exp { get; set; }
        public int api_mvp { get; set; }
        public int api_member_lv { get; set; }
        public int api_member_exp { get; set; }
        public int api_get_base_exp { get; set; }
        public int[] api_get_ship_exp { get; set; }
        public int[][] api_get_exp_lvup { get; set; }
        public int api_dests { get; set; }
        public int api_destsf { get; set; }
        public int[] api_lost_flag { get; set; }
        public string api_quest_name { get; set; }
        public int api_quest_level { get; set; }
        public class enemy_info
        {
            public string api_level { get; set; }
            public string api_rank { get; set; }
            public string api_deck_name { get; set; }
        }
        public enemy_info api_enemy_info { get; set; }
        public int api_first_clear { get; set; }
        public int api_mapcell_incentive { get; set; }
        public int[] api_get_flag { get; set; }
        public class get_useitem
        {
            public int api_useitem_id { get; set; }
            public string api_useitem_name { get; set; }
        }
        public get_useitem api_get_useitem { get; set; }
        public class get_ship
        {
            public int api_ship_id { get; set; }
            public string api_ship_type { get; set; }
            public string api_ship_name { get; set; }
            public string api_ship_getmes { get; set; }
        }
        public get_ship api_get_ship { get; set; }
        public class get_slotitem
        {
            public int api_slotitem_id { get; set; }
        }
        public class get_eventitem
        {
            public int api_type { get; set; }
            public int api_id { get; set; }
            public int api_value { get; set; }
        }
        public get_eventitem api_get_eventitem { get; set; }
        public int api_get_eventflag { get; set; }
        //public string api_get_exmap_rate { get; set; }
        //public string api_get_exmap_useitem_id { get; set; }
    }
}
