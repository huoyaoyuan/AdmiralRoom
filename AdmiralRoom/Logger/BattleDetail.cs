using System;
using Huoyaoyuan.AdmiralRoom.API;
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
        public class APILog<T>
        {
            public string api { get; set; }
            public svdata<T> data { get; set; }
        }
        public APILog<map_start_next> startnext { get; set; }
        public APILog<sortie_battle> battle { get; set; }
        public class ShipInfo
        {
            public int id { get; set; }
            public int shipid { get; set; }
            public int lv { get; set; }
            public int karyoku { get; set; }
            public int raisou { get; set; }
            public int taiku { get; set; }
            public int soukou { get; set; }
            public int kaihi { get; set; }
            public int taisen { get; set; }
            public int sakuteki { get; set; }
            public int lucky { get; set; }
            public EquipInfo[] slots { get; set; }
            public EquipInfo slotex { get; set; }
        }
        public class EquipInfo
        {
            public int itemid { get; set; }
            public int level { get; set; }
            public int alv { get; set; }
        }
        public ShipInfo[] fleet1 { get; set; }
        public ShipInfo[] fleet2 { get; set; }
    }
}
