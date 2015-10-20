using System;
using System.Timers;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class BuildingDock : GameObject<getmember_kdock>, IDisposable
    {
        public override int Id => rawdata.api_id;
        public DockState State { get; set; }
        public DateTime CompleteTime { get; set; }
        public DateTime CompleteTimeLocal => CompleteTime.ToLocalTime();
        public TimeSpan CompleteTimeRemain => CompleteTime.Remain();
        public TimeSpan DuringTime => CreatedShip?.BuildTime - CompleteTimeRemain ?? TimeSpan.FromSeconds(0);
        public int UseFuel => rawdata.api_item1;
        public int UseBull => rawdata.api_item2;
        public int UseSteel => rawdata.api_item3;
        public int UseBauxite => rawdata.api_item4;
        public int UseDevelopment => rawdata.api_item5;
        public ShipInfo CreatedShip { get; private set; }
        public bool IsLSC { get; set; }
        public Ship Secratary { get; set; }
        public BuildingDock()
        {
            Staff.Current.Ticker.Elapsed += Tick;
        }
        public BuildingDock(getmember_kdock api) : base(api)
        {
            Staff.Current.Ticker.Elapsed += Tick;
        }
        public void Dispose() => Staff.Current.Ticker.Elapsed -= Tick;
        private void Tick(object sender, ElapsedEventArgs e)
        {
            OnPropertyChanged("CompleteTimeRemain");
            OnPropertyChanged("DuringTime");
        }
        protected override void UpdateProp()
        {
            CompleteTime = DateTimeHelper.FromUnixTime(rawdata.api_complete_time);
            CreatedShip = Staff.Current.MasterData.ShipInfo[rawdata.api_created_ship_id];
            State = (DockState)rawdata.api_state;
            IsLSC = UseFuel > 999;
        }
    }
}
