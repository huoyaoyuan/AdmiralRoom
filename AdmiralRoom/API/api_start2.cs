namespace Huoyaoyuan.AdmiralRoom.API
{
    public class api_start2
    {
        public api_mst_ship[] api_mst_ship { get; set; }
        //public api_mst_shipgraph[] api_mst_shipgraph { get; set; }
        public api_mst_slotitem_equiptype[] api_mst_slotitem_equiptype { get; set; }
        //public object api_mst_equip_exslot { get; set; }
        public api_mst_stype[] api_mst_stype { get; set; }
        public api_mst_slotitem[] api_mst_slotitem { get; set; }
        //public api_mst_furniture[] api_mst_furniture { get; set; }
        //public api_mst_furnituregraph[] api_mst_furnituregraph { get; set; }
        public api_mst_useitem[] api_mst_useitem { get; set; }
        //public api_mst_payitem[] api_mst_payitem { get; set; }
        //public object api_mst_item_shop { get; set; }
        public api_mst_maparea[] api_mst_maparea { get; set; }
        public api_mst_mapinfo[] api_mst_mapinfo { get; set; }
        //public api_mst_mapbgm[] api_mst_mapbgm { get; set; }
        public api_mst_mapcell[] api_mst_mapcell { get; set; }
        public api_mst_mission[] api_mst_mission { get; set; }
        //public object api_mst_const { get; set; }
        public api_mst_shipupgrade[] api_mst_shipupgrade { get; set; }
        //public api_mst_bgm[] api_mst_bgm { get; set; }
    }
    public class api_mst_ship
    {
        public int api_id { get; set; }
        public int api_sortno { get; set; }
        public string api_name { get; set; }
        public string api_yomi { get; set; }
        public int api_stype { get; set; }
        public int api_afterlv { get; set; }
        public int api_aftershipid { get; set; }
        public int[] api_taik { get; set; }
        public int[] api_souk { get; set; }
        public int[] api_houg { get; set; }
        public int[] api_raig { get; set; }
        public int[] api_tyku { get; set; }
        public int[] api_luck { get; set; }
        public int api_soku { get; set; }
        public int api_leng { get; set; }
        public int api_slot_num { get; set; }
        public int[] api_maxeq { get; set; }
        public int api_buildtime { get; set; }
        public int[] api_broken { get; set; }
        public int[] api_powup { get; set; }
        public int api_backs { get; set; }
        public string api_getmes { get; set; }
        /// <summary>
        /// 这是钢！
        /// </summary>
        public int api_afterfuel { get; set; }
        public int api_afterbull { get; set; }
        public int api_fuel_max { get; set; }
        public int api_bull_max { get; set; }
        public int api_voicef { get; set; }
    }
    public class api_mst_slotitem_equiptype
    {
        public int api_id { get; set; }
        public string api_name { get; set; }
        public int api_show_flg { get; set; }
    }
    public class api_mst_stype
    {
        public int api_id { get; set; }
        public int api_sortno { get; set; }
        public string api_name { get; set; }
        public int api_scnt { get; set; }
        public int api_kcnt { get; set; }
        //public object[] api_equip_type { get; set; }
    }
    public class api_mst_slotitem
    {
        public int api_id { get; set; }
        public int api_sortno { get; set; }
        public string api_name { get; set; }
        public int[] api_type { get; set; }
        public int api_taik { get; set; }
        public int api_souk { get; set; }
        public int api_houg { get; set; }
        public int api_raig { get; set; }
        public int api_soku { get; set; }
        public int api_baku { get; set; }
        public int api_tyku { get; set; }
        public int api_tais { get; set; }
        public int api_atap { get; set; }
        public int api_houm { get; set; }
        public int api_raim { get; set; }
        public int api_houk { get; set; }
        public int api_raik { get; set; }
        public int api_bakk { get; set; }
        public int api_saku { get; set; }
        public int api_sakb { get; set; }
        public int api_luck { get; set; }
        public int api_leng { get; set; }
        public int api_rare { get; set; }
        public int[] api_broken { get; set; }
        public string api_info { get; set; }
        public int api_usebull { get; set; }
    }
    public class api_mst_useitem
    {
        public int api_id { get; set; }
        public int api_usetype { get; set; }
        public int api_category { get; set; }
        public string api_name { get; set; }
        public string[] api_description { get; set; }
        public int api_price { get; set; }
    }
    public class api_mst_maparea
    {
        public int api_id { get; set; }
        public string api_name { get; set; }
        public int api_type { get; set; }
    }
    public class api_mst_mapinfo
    {
        public int api_id { get; set; }
        public int api_maparea_id { get; set; }
        public int api_no { get; set; }
        public string api_name { get; set; }
        public int api_level { get; set; }
        public string api_opetext { get; set; }
        public string api_infotext { get; set; }
        public int[] api_item { get; set; }
        public int? api_max_maphp { get; set; }
        public int? api_required_defeat_count { get; set; }
        public int[] api_sally_flag { get; set; }
    }
    public class api_mst_mapcell
    {
        public int api_mapno { get; set; }
        public int api_maparea_id { get; set; }
        public int api_mapinfo_id { get; set; }
        public int api_id { get; set; }
        public int api_no { get; set; }
        public int api_color_no { get; set; }
    }
    public class api_mst_mission
    {
        public int api_id { get; set; }
        public int api_maparea_id { get; set; }
        public string api_name { get; set; }
        public string api_details { get; set; }
        public int api_time { get; set; }
        public int api_difficulty { get; set; }
        public decimal api_use_fuel { get; set; }
        public decimal api_use_bull { get; set; }
        public int[] api_win_item1 { get; set; }
        public int[] api_win_item2 { get; set; }
        public int api_return_flag { get; set; }
    }
    public class api_mst_shipupgrade
    {
        public int api_id { get; set; }
        public int api_current_ship_id { get; set; }
        public int api_original_ship_id { get; set; }
        public int api_upgrade_type { get; set; }
        public int api_upgrade_level { get; set; }
        public int api_drawing_count { get; set; }
        public int api_catapult_count { get; set; }
        public int api_sortno { get; set; }
    }
}
