using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huoyaoyuan.AdmiralRoom
{
    static class DateTimeHelper
    {
        public static DateTime FromUnixTime(long unixtime)
        {
            DateTime starttime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return starttime.AddMilliseconds(unixtime);
        }
    }
}
