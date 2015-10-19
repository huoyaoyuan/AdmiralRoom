using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huoyaoyuan.AdmiralRoom.API
{
    public class kousyou_createitem
    {
        public int api_create_flag { get; set; }
        public int api_shizai_flag { get; set; }
        public string api_fdata { get; set; }
        public int[] api_material { get; set; }
        public getmember_slotitem api_slot_item { get; set; }
        public int api_type3 { get; set; }
        //public object[] api_unsetslot { get; set; }
    }
}
