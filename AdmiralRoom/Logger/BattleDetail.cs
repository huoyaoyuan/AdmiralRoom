using System;
using Meowtrix;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    class BattleDetail : IIdentifiable<DateTime>
    {
        public string time { get; set; }
        private DateTime? _utctime;
        public DateTime GetTimeStamp()
        {
            if (_utctime == null)
            {
                var utc = DateTime.Parse(time);
                _utctime = DateTime.SpecifyKind(utc, DateTimeKind.Unspecified);
            }
            return _utctime.Value;
        }
        DateTime IIdentifiable<DateTime>.Id => GetTimeStamp();
    }
}
