namespace Huoyaoyuan.AdmiralRoom.API
{
    public class ranking_getlist
    {
        public int api_count { get; set; }
        public int api_page_count { get; set; }
        public int api_disp_page { get; set; }
        public ranking_list[] api_list { get; set; }
        public class ranking_list
        {
            public int api_no { get; set; }
            public int api_member_id { get; set; }
            public int api_level { get; set; }
            public int api_rank { get; set; }
            public string api_nickname { get; set; }
            public int api_experience { get; set; }
            public string api_comment { get; set; }
            public int api_rate { get; set; }
            public int api_flag { get; set; }
            public int api_medals { get; set; }
        }
    }
}
