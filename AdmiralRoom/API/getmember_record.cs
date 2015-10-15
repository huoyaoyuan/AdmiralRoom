using System;

namespace Huoyaoyuan.AdmiralRoom.API
{
    public class getmember_record
    {
        public int api_member_id { get; set; }
        public string api_nickname { get; set; }
        public int api_nickname_id { get; set; }
        public string api_cmt { get; set; }
        public int api_cmt_id { get; set; }
        public string api_photo_url { get; set; }
        public int api_level { get; set; }
        public int api_rank { get; set; }
        public int[] api_experience { get; set; }
        public api_war api_war { get; set; }
        public api_mission api_mission { get; set; }
        public api_war api_practice { get; set; }
        public int api_friend { get; set; }
        public int api_deck { get; set; }
        public int api_kdoc { get; set; }
        public int api_ndoc { get; set; }
        public int[] api_ship { get; set; }
        public int[] api_slotitem { get; set; }
        public int api_furniture { get; set; }
        public decimal[] api_complate { get; set; }
        public int api_large_dock { get; set; }
        public int api_material_max { get; set; }
    }
    
    public struct api_war
    {
        public int api_win { get; set; }
        public int api_lose { get; set; }
        public decimal api_rate { get; set; }
    }
    
    public struct api_mission
    {
        public int api_count { get; set; }
        public int api_success { get; set; }
        public decimal api_rate { get; set; }
    }
}
