namespace Huoyaoyuan.AdmiralRoom.API
{
    public class port_port
    {
        public getmember_material[] api_material { get; set; }
        public getmember_deck[] api_deck_port { get; set; }
        public getmember_ndock[] api_ndock { get; set; }
        public api_ship[] api_ship { get; set; }
        public getmember_basic api_basic { get; set; }
        public api_log[] api_log { get; set; }
        public int api_combined_flag { get; set; }
        public int api_p_bgm_id { get; set; }
        //public object api_event_object;
        public int api_parallel_quest_count { get; set; }
    }
    
    public class api_log
    {
        public int api_no { get; set; }
        public int api_type { get; set; }//1から 入渠, 工廠, 遠征, 支給?, 演習, 勲章?, 出撃, 任務?, 申請?, 昇格?, 図鑑, 達成?,
        public int api_state { get; set; }
        public string api_message { get; set; }
    }
}
