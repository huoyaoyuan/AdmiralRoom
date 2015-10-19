using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huoyaoyuan.AdmiralRoom.API
{
    public class kaisou_powerup
    {
        public int api_powerup_flag { get; set; }
        public api_ship[] api_ship { get; set; }
        public getmember_deck[] api_deck { get; set; }
    }
}
