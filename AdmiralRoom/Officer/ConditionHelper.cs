using System;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class ConditionHelper
    {
        public static ConditionHelper Instance { get; } = new ConditionHelper();
        private ConditionHelper() { }
        private int maxup;
        private DateTime increasefrom, increaseto, lastcheck, brokentime;
        private bool broken = true;
        private bool updating;
        private readonly TimeSpan maxerror = TimeSpan.FromSeconds(2);
        public void OnCondition(int d)
        {
            if (updating)
                maxup = d > maxup ? d : maxup;
        }
        public void BeginUpdate()
        {
            updating = true;
            maxup = 0;
        }
        public void EndUpdate()
        {
            updating = false;
            var checktime = DateTime.UtcNow;
            if (brokentime == DateTime.MinValue)//first time
            {
                lastcheck = checktime;
                brokentime = checktime;
                return;
            }
            if (maxup == 0)
            {
                lastcheck = checktime;
                return;
            }
            if (broken)
            {
                increasefrom = lastcheck;
                increaseto = checktime;
                broken = false;
                return;
            }
            maxup = (int)Math.Ceiling(maxup / 3.0) * 3;
            increasefrom += TimeSpan.FromMinutes(maxup);
            increaseto += TimeSpan.FromMinutes(maxup);
            while (increaseto + maxerror < lastcheck)
            {
                increasefrom += TimeSpan.FromMinutes(3);
                increaseto += TimeSpan.FromMinutes(3);
            }
            //if (increasefrom > checktime + maxerror)//broken
            //{
            //    maxup = 0;
            //    lastcheck = checktime;
            //    broken = true;
            //    brokentime = checktime;
            //    return;
            //}
            if (increasefrom < lastcheck) increasefrom = lastcheck;
            if (increaseto > checktime) increaseto = checktime;
            if (increasefrom > increaseto)//swap
            {
                var temp = increasefrom;
                increasefrom = increaseto;
                increaseto = temp;
            }
            broken = false;
            lastcheck = checktime;
        }
        public DateTime BaseTime
        {
            get
            {
                if (broken) return brokentime;
                else return increaseto;
            }
        }
        public TimeSpan Offset
        {
            get
            {
                if (broken) return TimeSpan.FromMinutes(3);
                else return increaseto - increasefrom;
            }
        }
        public TimeSpan Remain(int cond) => (BaseTime + TimeSpan.FromMinutes((int)Math.Ceiling((40 - cond) / 3.0) * 3)).Remain();
    }
}
