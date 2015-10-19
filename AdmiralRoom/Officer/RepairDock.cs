using System;
using System.Timers;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class RepairDock : GameObject<getmember_ndock>, IDisposable
    {
        public override int Id => rawdata.api_id;
        public DockState State => (DockState)rawdata.api_state;
        public DateTime CompleteTime { get; private set; }
        public DateTime CompleteTimeLocal => CompleteTime.ToLocalTime();
        public TimeSpan CompleteTimeRemain => CompleteTime.Remain();
        public Ship Ship { get; private set; }
        public int UseFuel => rawdata.api_item1;
        public int UseSteel => rawdata.api_item3;
        public RepairDock()
        {
            Staff.Current.Ticker.Elapsed += Tick;
        }
        public RepairDock(getmember_ndock api) : base(api)
        {
            Staff.Current.Ticker.Elapsed += Tick;
        }
        public void Dispose() => Staff.Current.Ticker.Elapsed -= Tick;
        private void Tick(object sender, ElapsedEventArgs e) => OnPropertyChanged("CompleteTimeRemain");
        protected override void UpdateProp()
        {
            CompleteTime = DateTimeHelper.FromUnixTime(rawdata.api_complete_time);
            Ship = Staff.Current.Homeport.Ships[rawdata.api_ship_id];
        }
    }
    public enum DockState { Locked = -1, Empty = 0, Repairing = 1, Building = 2, BuildComplete = 3 }
}
