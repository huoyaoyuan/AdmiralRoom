using System;
using System.Timers;
using System.Windows;
using Huoyaoyuan.AdmiralRoom.API;
using Huoyaoyuan.AdmiralRoom.Properties;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class RepairDock : GameObject<getmember_ndock>
    {
        public override int Id => rawdata.api_id;
        public DockState State => (DockState)rawdata.api_state;
        public DateTimeOffset CompleteTime { get; private set; }
        public Ship Ship { get; private set; }
        public int UseFuel => rawdata.api_item1;
        public int UseSteel => rawdata.api_item3;
        public RepairDock()
        {
            WeakEventManager<Timer, ElapsedEventArgs>.AddHandler(Staff.Current.Ticker, "Elapsed", Tick);
        }
        public RepairDock(getmember_ndock api) : base(api)
        {
            WeakEventManager<Timer, ElapsedEventArgs>.AddHandler(Staff.Current.Ticker, "Elapsed", Tick);
        }
        private void Tick(object sender, ElapsedEventArgs e)
        {
            OnPropertyChanged("CompleteTime");
            if (Ship != null)
                Ship.RepairingHP = Ship.HP.Max - (int)Math.Ceiling(CompleteTime.Remain().TotalSeconds / Ship.RepairTimePerHP.TotalSeconds);
            if (State == DockState.Repairing && Config.Current.NotifyWhenRepair && CompleteTime.InASecond(Config.Current.NotifyTimeAdjust))
                Notifier.Current?.Show(Resources.Notification_Repair_Title, string.Format(Resources.Notification_Repair_Text, Id, Ship.ShipInfo.Name));
        }
        protected override void UpdateProp()
        {
            CompleteTime = DateTimeOffset.FromUnixTimeMilliseconds(rawdata.api_complete_time);
            if (State == DockState.Repairing)
                Ship = Staff.Current.Homeport.Ships[rawdata.api_ship_id];
            else Ship = null;
        }
    }
    public enum DockState { Locked = -1, Empty = 0, Repairing = 1, Building = 2, BuildComplete = 3 }
}
