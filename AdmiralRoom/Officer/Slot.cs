using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Slot
    {
        public bool IsLocked { get; set; }
        public Equipment Item { get; set; }
        public LimitedValue AirCraft { get; set; }
        public bool HasItem => Item != null;
    }
}
