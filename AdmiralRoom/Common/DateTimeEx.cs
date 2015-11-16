using System;

namespace Huoyaoyuan.AdmiralRoom
{
    static class DateTimeEx
    {
        public static TimeSpan Remain(this DateTimeOffset time)
        {
            if (time.ToLocalTime() > DateTimeOffset.Now)
                return time - DateTimeOffset.Now;
            else return new TimeSpan(0);
        }
        public static DateTimeOffset WeekStart(this DateTimeOffset time)
        {
            try
            {
                int dayofweek = (int)time.DayOfWeek - 1;
                if (dayofweek < 0) dayofweek += 7;
                return time.Date.AddDays(-dayofweek);
            }
            catch { return DateTimeOffset.MinValue; }
        }
    }
}
