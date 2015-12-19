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
        public static TimeSpan During(this DateTimeOffset time)
        {
            if (time.ToLocalTime() < DateTimeOffset.Now)
                return DateTimeOffset.Now - time;
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
        public static bool InASecond(this TimeSpan time, double adjust = 0)
        {
            double seconds = time.TotalSeconds;
            return seconds > adjust && seconds <= adjust + 1;
        }
        public static bool InASecond(this DateTimeOffset time, double adjust = 0) => (time - DateTimeOffset.UtcNow).InASecond(adjust);
    }
}
