using System;
using System.Timers;
using System.Windows;
using Huoyaoyuan.AdmiralRoom.API;
using Meowtrix;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class BuildingDock : GameObject<getmember_kdock>
    {
        public override int Id => rawdata.api_id;
        public DockState State { get; set; }
        public DateTimeOffset CompleteTime { get; set; }
        public TimeSpan DuringTime => CreatedShip?.BuildTime - CompleteTime.Remain() ?? TimeSpan.FromSeconds(0);
        public int UseFuel => rawdata.api_item1;
        public int UseBull => rawdata.api_item2;
        public int UseSteel => rawdata.api_item3;
        public int UseBauxite => rawdata.api_item4;
        public int UseDevelopment => rawdata.api_item5;
        public ShipInfo CreatedShip { get; private set; }
        public bool IsLSC { get; set; }
        public int HighSpeed { get; set; }
        public Ship Secretary { get; set; }
        public BuildingDock(getmember_kdock api) : base(api)
        {
            WeakEventManager<Timer, ElapsedEventArgs>.AddHandler(Staff.Current.Ticker, "Elapsed", Tick);
        }
        private void Tick(object sender, ElapsedEventArgs e)
        {
            OnPropertyChanged(nameof(CompleteTime));
            OnPropertyChanged(nameof(DuringTime));
            if (State == DockState.Building && Config.Current.NotifyWhenBuild && CompleteTime.InASecond())
                Notifier.Current?.Show(StringTable.Notification_Build_Title,
                    string.Format(StringTable.Notification_Build_Text, Id, Config.Current.ShowBuildingShipName ? $"「{CreatedShip.Name}」" : StringTable.Ship_LowerCase),
Config.MakeSoundWithPath(Config.Current.NotifyBuildSound));
        }
        protected override void UpdateProp()
        {
            CompleteTime = DateTimeOffset.FromUnixTimeMilliseconds(rawdata.api_complete_time);
            CreatedShip = Staff.Current.MasterData.ShipInfo[rawdata.api_created_ship_id];
            State = (DockState)rawdata.api_state;
            IsLSC = UseFuel > 999;
        }
    }
}
