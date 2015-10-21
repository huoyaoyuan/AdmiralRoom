using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huoyaoyuan.AdmiralRoom.API
{
    public class getmember_questlist
    {
        public int api_count { get; set; }
        public int api_page_count { get; set; }
        public int api_disp_page { get; set; }
        public api_quest[] api_list { get; set; }
        public int api_exec_count { get; set; }
        public int api_exec_type { get; set; }
    }
    public class api_quest
    {
        public int api_no { get; set; }
        public int api_category { get; set; }
        public int api_type { get; set; }
        public int api_state { get; set; }
        public string api_title { get; set; }
        public string api_detail { get; set; }
        public int[] api_get_material { get; set; }
        public int api_bonus_flag { get; set; }
        public int api_progress_flag { get; set; }
        public int api_invalid_flag { get; set; }
    }
}
