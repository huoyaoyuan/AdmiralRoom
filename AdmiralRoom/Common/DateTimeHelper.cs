using System;

namespace Huoyaoyuan.AdmiralRoom
{
    static class DateTimeHelper
    {
        public static DateTime FromUnixTime(long unixtime)
        {
            DateTime starttime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return starttime.AddMilliseconds(unixtime);
        }
        public static TimeSpan Remain(this DateTime time)
        {
            if (time.ToLocalTime() > DateTime.Now)
                return time.ToLocalTime() - DateTime.Now;
            else return new TimeSpan(0);
        }
        public static DateTime WeekStart(this DateTime time)
        {
            int dayofweek = (int)time.DayOfWeek - 1;
            if (dayofweek < 0) dayofweek += 7;
            return time.Date.AddDays(-dayofweek);
        }
    }
}
