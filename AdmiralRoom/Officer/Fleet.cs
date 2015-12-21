using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Fleet : GameObject<getmember_deck>, IDisposable
    {
        public override int Id => rawdata.api_id;
        public string Name => rawdata.api_name;
        public FleetMissionState MissionState => (FleetMissionState)rawdata.api_mission[0];
        public int MissionID => (int)rawdata.api_mission[1];
        public MissionInfo MissionInfo => Staff.Current.MasterData.MissionInfo[MissionID];
        public DateTimeOffset BackTime { get; private set; }
        public TimeSpan ConditionTimeRemain => ConditionHelper.Instance.Remain(mincondition);
        public int ConditionTimeOffset => (int)ConditionHelper.Instance.Offset.TotalSeconds;

        #region Ships
        private ObservableCollection<Ship> _ships = new ObservableCollection<Ship>();
        public ObservableCollection<Ship> Ships
        {
            get { return _ships; }
            set
            {
                if (_ships != value)
                {
                    _ships = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public Fleet() { }
        public Fleet(getmember_deck api) : base(api)
        {
            Staff.Current.Ticker.Elapsed += Tick;
            Staff.Current.Ticker.Elapsed += CheckHomeportRepairing;
        }
        public void Dispose()
        {
            Staff.Current.Ticker.Elapsed -= Tick;
            Staff.Current.Ticker.Elapsed -= CheckHomeportRepairing;
            Ships.ForEach(x => x.InFleet = null);
        }
        private void Tick(object sender, ElapsedEventArgs e)
        {
            OnPropertyChanged("BackTime");
            OnPropertyChanged("ConditionTimeRemain");
            OnPropertyChanged("HomeportRepairingFrom");
            if (MissionState == FleetMissionState.InMission && Config.Current.NotifyWhenExpedition && BackTime.InASecond(Config.Current.NotifyTimeAdjust))
                Notifier.Current?.Show("远征归来", $"{Name} 远征归来：远征{MissionID} - {MissionInfo.Name}");
            if (!InSortie && Config.Current.NotifyWhenCondition && ConditionTimeRemain.InASecond())
                Notifier.Current?.Show("疲劳恢复完毕", $"{Name} 疲劳恢复完毕。");
        }

        public enum FleetMissionState { None = 0, InMission = 1, Complete = 2, Abort = 3 }

        #region NeedCharge
        private bool _needcharge;
        public bool NeedCharge
        {
            get { return _needcharge; }
            set
            {
                if (_needcharge != value)
                {
                    _needcharge = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region LowCondition
        private bool _lowcondition;
        public bool LowCondition
        {
            get { return _lowcondition; }
            set
            {
                if (_lowcondition != value)
                {
                    _lowcondition = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region HeavilyDamaged
        private bool _heavilydamaged;
        public bool HeavilyDamaged
        {
            get { return _heavilydamaged; }
            set
            {
                if (_heavilydamaged != value)
                {
                    _heavilydamaged = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Repairing
        private bool _repairing;
        public bool Repairing
        {
            get { return _repairing; }
            set
            {
                if (_repairing != value)
                {
                    _repairing = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Status
        private FleetStatus _status;
        public FleetStatus Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region InSortie
        private bool _insortie;
        public bool InSortie
        {
            get { return _insortie; }
            set
            {
                if (_insortie != value)
                {
                    _insortie = value;
                    UpdateStatus();
                }
            }
        }
        #endregion

        private bool needupdateship = false;
        protected override void UpdateProp()
        {
            BackTime = DateTimeOffset.FromUnixTimeMilliseconds(rawdata.api_mission[2]);
            needupdateship = false;
            for (int i = 0; i < rawdata.api_ship.Length; i++)
            {
                if (rawdata.api_ship[i] == -1)
                {
                    if (Ships.Count > i)
                    {
                        needupdateship = true;
                        break;
                    }
                }
                else
                {
                    if (Ships.Count <= i || Ships[i].Id != rawdata.api_ship[i] || Ships[i].InFleet != this)
                    {
                        needupdateship = true;
                        break;
                    }
                }
            }
            if (needupdateship)
                Ships = new ObservableCollection<Ship>(rawdata.api_ship
                    .Where(x => x != -1)
                    .Select(x =>
                    {
                        if (x == -1) return null;
                        Staff.Current.Homeport.Ships[x].InFleet = this;
                        return Staff.Current.Homeport.Ships[x];
                    }));
            UpdateStatus();
        }
        public int[] AirFightPower { get; private set; }
        public int LevelSum => Ships.Select(x => x.Level).Sum();
        public double LevelAverage => Ships.Any() ? Ships.Select(x => (double)x.Level).Average() : 0;
        public double LoSInMap => Ships.Select(x => x.LoSInMap).Sum() - Math.Ceiling(Staff.Current.Admiral.Level / 5.0) * 5.0 * 0.61;
        public int[] ChargeCost => new[]
        {
            Ships.Select(x => x.ApplyMarriage(x.Fuel.Shortage)).Sum(),
            Ships.Select(x => x.ApplyMarriage(x.Bull.Shortage)).Sum(),
            Ships.Select(x => x.Slots.Select(y => y.AirCraft.Shortage).Sum()).Sum() * 5
        };
        public int[] RepairCost => new[] { Ships.Select(x => x.RepairFuel).Sum(), Ships.Select(x => x.RepairSteel).Sum() };
        private int mincondition = 49;
        public void UpdateStatus()
        {
            bool f1 = false, f2 = false, f3 = false, f4 = false;
            foreach (var ship in Ships)
            {
                if (ship.Condition < 40) f1 = true;
                if (ship.HP.Current * 4 <= ship.HP.Max) f2 = true;
                if (!(ship.Fuel.IsMax && ship.Bull.IsMax)) f3 = true;
                if (ship.IsRepairing) f4 = true;
            }
            LowCondition = f1;
            HeavilyDamaged = f2;
            NeedCharge = f3;
            Repairing = f4;
            if (InSortie)
                if (HeavilyDamaged) Status = FleetStatus.Warning;
                else Status = FleetStatus.InSortie;
            else if (MissionState != FleetMissionState.None)
                Status = FleetStatus.InMission;
            else if (NeedCharge || HeavilyDamaged || LowCondition || Repairing)
                Status = FleetStatus.NotReady;
            else Status = FleetStatus.Ready;
            AirFightPower = Ships.Aggregate(new int[8], (x, y) => x.Zip(y.AirFightPower, (a, b) => a + b).ToArray());
            if (Ships.Any())
                mincondition = Ships.Select(x => x.Condition).Min();
            else
            {
                mincondition = 49;
                Status = FleetStatus.Empty;
            }
            OnPropertyChanged("AirFightPower");
            OnPropertyChanged("LevelSum");
            OnPropertyChanged("LevelAverage");
            OnPropertyChanged("LoSInMap");
            OnPropertyChanged("ChargeCost");
            OnPropertyChanged("RepairCost");
            OnPropertyChanged("CanHomeportRepairing");
        }

        protected override void OnAllPropertyChanged()
        {
            OnPropertyChanged("Name");
            OnPropertyChanged("MissionState");
            OnPropertyChanged("MissionID");
            OnPropertyChanged("MissionInfo");
            OnPropertyChanged("BackTime");
            OnPropertyChanged("ConditionTimeRemain");
            OnPropertyChanged("ConditionTimeOffset");
            if (needupdateship) OnPropertyChanged("Ships");
        }

        public bool CanHomeportRepairing
            => Ships.Count > 0
            && Ships[0].ShipInfo.ShipType.Id == 19
            && Ships[0].IsRepairing == false
            && MissionState == FleetMissionState.None;
        private IEnumerable<Ship> HomeportRepairingList => Ships.Take(
            CanHomeportRepairing ? Ships[0].Slots.Where(x => x.Item?.EquipInfo.EquipType.Id == 31).Count() + 2 : 0)
            .Where(x => x.HP.Current * 2 > x.HP.Max);

        public DateTimeOffset HomeportRepairingFrom { get; private set; }
        public void CheckHomeportRepairingTime(bool reset)
        {
            var time = DateTimeOffset.UtcNow;
            if (reset || (time - HomeportRepairingFrom).TotalMinutes >= 20) HomeportRepairingFrom = time;
            CheckHomeportRepairing(null, null);
        }

        private void CheckHomeportRepairing(object sender, ElapsedEventArgs e)
        {
            var during = DateTimeOffset.UtcNow - HomeportRepairingFrom;
            if (during.TotalMinutes < 20) return;
            foreach (var ship in HomeportRepairingList)
                if (!ship.IsRepairing)
                    ship.RepairingHP = ship.HP.Current + Math.Max((int)((during.TotalSeconds - 30) / ship.RepairTimePerHP.TotalSeconds), 1);
        }
    }
    public enum FleetStatus { Empty, Ready, NotReady, InSortie, InMission, Warning }
}
