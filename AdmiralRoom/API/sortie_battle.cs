namespace Huoyaoyuan.AdmiralRoom.API
{
    public class sortie_battle
    {
        public int[] api_active_deck { get; set; }
        public int api_deck_id { get; set; }
        public int[] api_ship_ke { get; set; }
        public int[] api_ship_ke_combined { get; set; }
        public int[] api_ship_lv { get; set; }
        public int[] api_ship_lv_combined { get; set; }
        public int[] api_f_nowhps { get; set; }
        public int[] api_f_maxhps { get; set; }
        public int[] api_f_nowhps_combined { get; set; }
        public int[] api_f_maxhps_combined { get; set; }
        public int[] api_e_nowhps { get; set; }
        public int[] api_e_maxhps { get; set; }
        public int[] api_e_nowhps_combined { get; set; }
        public int[] api_e_maxhps_combined { get; set; }
        public int api_midnight_flag { get; set; }
        public int[][] api_eSlot { get; set; }
        public int[][] api_eSlot_combined { get; set; }
        public int[][] api_fParam { get; set; }
        public int[][] api_eParam { get; set; }
        public int[][] api_fParam_combined { get; set; }
        public int[][] api_eParam_combined { get; set; }
        public int[] api_escape_idx { get; set; }
        public int[] api_escape_idx_combined { get; set; }
        public int[] api_search { get; set; }
        public int[] api_formation { get; set; }
        public airbattle api_air_base_injection { get; set; }
        public airbattle api_injection_kouku { get; set; }
        public airbattle[] api_air_base_attack { get; set; }
        public int[] api_stage_flag { get; set; }
        public class airbattle
        {
            public int api_base_id { get; set; }
            /// <summary>
            /// 1基
            /// </summary>
            public int[][] api_plane_from { get; set; }
            public class squadron
            {
                public int api_mst_id { get; }
                public int api_count { get; }
            }
            public squadron[] api_squadron_plane { get; set; }
            public class stage1
            {
                public int api_f_count { get; set; }
                public int api_f_lostcount { get; set; }
                public int api_e_count { get; set; }
                public int api_e_lostcount { get; set; }
                public int api_disp_seiku { get; set; }
                public int[] api_touch_plane { get; set; }
            }
            public stage1 api_stage1 { get; set; }
            public class stage2
            {
                public int api_f_count { get; set; }
                public int api_f_lostcount { get; set; }
                public int api_e_count { get; set; }
                public int api_e_lostcount { get; set; }
                public class anti_air_cutin
                {
                    public int api_idx { get; set; }
                    public int api_kind { get; set; }
                    public int[] api_use_items { get; set; }
                }
                public anti_air_cutin api_air_fire { get; set; }
            }
            public stage2 api_stage2 { get; set; }
            public class stage3
            {
                public int[] api_erai_flag { get; set; }
                public int[] api_ebak_flag { get; set; }
                public int[] api_ecl_flag { get; set; }
                public decimal[] api_edam { get; set; }
                public int[] api_frai_flag { get; set; }
                public int[] api_fbak_flag { get; set; }
                public int[] api_fcl_flag { get; set; }
                public decimal[] api_fdam { get; set; }
            }
            public stage3 api_stage3 { get; set; }
            public stage3 api_stage3_combined { get; set; }
        }
        public airbattle api_kouku { get; set; }
        public airbattle api_kouku2 { get; set; }
        public int api_support_flag { get; set; }
        public class support
        {
            public interface Isupportinfo
            {
                int api_deck_id { get; }
                int[] api_ship_id { get; }
                int[] api_undressing_flag { get; }
            }
            public class support_air : airbattle, Isupportinfo
            {
                public int api_deck_id { get; set; }
                public int[] api_ship_id { get; set; }
                public int[] api_undressing_flag { get; set; }
                public int[] api_stage_flag { get; set; }
            }
            public support_air api_support_airatack { get; set; }
            public class support_hourai : Isupportinfo
            {
                public int api_deck_id { get; set; }
                public int[] api_ship_id { get; set; }
                public int[] api_undressing_flag { get; set; }
                public int[] api_cl_list { get; set; }
                public decimal[] api_damage { get; set; }
            }
            public support_hourai api_support_hourai { get; set; }
        }
        public support api_support_info { get; set; }
        public int api_n_support_flag { get; set; }
        public support api_n_support_info { get; set; }
        public int api_opening_taisen_flag { get; set; }
        public fire api_opening_taisen { get; set; }
        public int api_opening_flag { get; set; }
        public class torpedo
        {
            public int[] api_frai { get; set; }
            public int[] api_erai { get; set; }
            public decimal[] api_fdam { get; set; }
            public decimal[] api_edam { get; set; }
            public decimal[] api_fydam { get; set; }
            public decimal[] api_eydam { get; set; }
            public int[] api_fcl { get; set; }
            public int[] api_ecl { get; set; }
        }
        public torpedo api_opening_atack { get; set; }
        public int[] api_hourai_flag { get; set; }
        public class fire
        {
            public int[] api_at_eflag { get; set; }
            public int[] api_at_list { get; set; }
            /// <summary>
            /// 昼战特殊攻击
            /// </summary>
            public int[] api_at_type { get; set; }
            /// <summary>
            /// 夜战特殊攻击
            /// </summary>
            public int[] api_sp_list { get; set; }
            public int[][] api_df_list { get; set; }
            public int[][] api_si_list { get; set; }
            public int[][] api_cl_list { get; set; }
            public decimal[][] api_damage { get; set; }
        }
        public fire api_hougeki1 { get; set; }
        public fire api_hougeki2 { get; set; }
        public fire api_hougeki3 { get; set; }
        public torpedo api_raigeki { get; set; }
        /// <summary>
        /// 夜間触接
        /// </summary>
        public int[] api_touch_plane { get; set; }
        /// <summary>
        /// 照明弾発射艦 [0]=味方, [1]=敵　敵でも1-6
        /// </summary>
        public int[] api_flare_pos { get; set; }
        /// <summary>
        /// 夜戦
        /// </summary>
        public fire api_hougeki { get; set; }
        public fire api_n_hougeki1 { get; set; }
        public fire api_n_hougeki2 { get; set; }
    }
}
