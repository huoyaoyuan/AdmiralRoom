using System;

namespace Huoyaoyuan.AdmiralRoom.API
{
    public class getmember_basic
    {
        public int api_member_id { get; set; }
        public string api_nickname { get; set; }
        //public int api_nickname_id { get; set; }
        //public int api_active_flag;
        public long api_starttime { get; set; }
        public int api_level { get; set; }
        public int api_rank { get; set; }
        public int api_experience { get; set; }
        public string api_fleetname { get; set; }
        public string api_comment { get; set; }
        //public int api_comment_id { get; set; }
        public int api_max_chara { get; set; }
        public int api_max_slotitem { get; set; }
        public int api_max_kagu { get; set; }
        public int api_count_deck { get; set; }
        public int api_count_kdock { get; set; }
        public int api_count_ndock { get; set; }
        public int api_fcoin { get; set; }
        public int api_st_win { get; set; }
        public int api_st_lose { get; set; }
        public int api_ms_count { get; set; }
        public int api_ms_success { get; set; }
        public int api_pt_win { get; set; }
        public int api_pt_lose { get; set; }
        public int api_pt_challenged { get; set; }
        public int api_pt_challenged_win { get; set; }
        public int api_firstflag { get; set; }
        public int api_tutorial_progress { get; set; }
        public int[] api_pvp { get; set; }
        public int api_medals { get; set; }
    }
}
