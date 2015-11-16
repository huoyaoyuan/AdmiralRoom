using System;
using System.Timers;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class RepairDock : GameObject<getmember_ndock>, IDisposable
    {
        public override int Id => rawdata.api_id;
        public DockState State => (DockState)rawdata.api_state;
        public DateTimeOffset CompleteTime { get; private set; }
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
        private void Tick(object sender, ElapsedEventArgs e) => OnPropertyChanged("CompleteTime");
        protected override void UpdateProp()
        {
            CompleteTime = DateTimeOffset.FromUnixTimeMilliseconds(rawdata.api_complete_time);
            if (State == DockState.Repairing)
                Ship = Staff.Current.Homeport.Ships[rawdata.api_ship_id];
        }
    }
    public enum DockState { Locked = -1, Empty = 0, Repairing = 1, Building = 2, BuildComplete = 3 }
}
