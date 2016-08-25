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
            /// <summary>
            /// api_no
            /// </summary>
            public int api_mxltvkpyuklh { get; set; }
            //public int api_member_id { get; set; }
            //public int api_level { get; set; }
            //public int api_rank { get; set; }
            /// <summary>
            /// api_nickname
            /// </summary>
            public string api_mtjmdcwtvhdr { get; set; }
            //public int api_experience { get; set; }
            /// <summary>
            /// api_comment
            /// </summary>
            public string api_itbrdpdbkynm { get; set; }
            /// <summary>
            /// api_rate
            /// </summary>
            public int api_wuhnhojjxmke { get; set; }
            //public int api_flag { get; set; }
            //public int api_medals { get; set; }
        }
    }
}
