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
        public DateTime BackTime { get; private set; }
        public DateTime BackTimeLocal => BackTime.ToLocalTime();
        public TimeSpan BackTimeRemain => BackTime.Remain();

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
        }
        public void Dispose() => Staff.Current.Ticker.Elapsed -= Tick;
        private void Tick(object sender, ElapsedEventArgs e) => OnPropertyChanged("BackTimeRemain");

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
            BackTime = DateTimeEx.FromUnixTime(rawdata.api_mission[2]);
            needupdateship = false;
            for(int i = 0; i < rawdata.api_ship.Length; i++)
            {
                if (rawdata.api_ship[i] == -1)
                {
                    if(Ships.Count > i)
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
                Ships = new ObservableCollection<Ship>(rawdata.api_ship.ArrayOperation(x =>
                {
                    if (x == -1) return null;
                    Staff.Current.Homeport.Ships[x].InFleet = this;
                    return Staff.Current.Homeport.Ships[x];
                }));
            UpdateStatus();
        }
        public int[] AirFightPower { get; private set; }
        public void UpdateStatus()
        {
            bool f1 = false, f2 = false, f3 = false;
            foreach (var ship in Ships)
            {
                if (ship.Condition < 40) f1 = true;
                if (ship.HP.Current * 4 <= ship.HP.Max) f2 = true;
                if (!(ship.Fuel.IsMax && ship.Bull.IsMax)) f3 = true;
            }
            LowCondition = f1;
            HeavilyDamaged = f2;
            NeedCharge = f3;
            if (InSortie)
                if (HeavilyDamaged) Status = FleetStatus.Warning;
                else Status = FleetStatus.InSortie;
            else if (MissionState != FleetMissionState.None)
                Status = FleetStatus.InMission;
            else if (NeedCharge || HeavilyDamaged || LowCondition)
                Status = FleetStatus.NotReady;
            else Status = FleetStatus.Ready;
            AirFightPower = new int[8];
            foreach (var ship in Ships)
                for (int i = 0; i < 8; i++)
                    AirFightPower[i] += ship.AirFightPower[i];
            OnPropertyChanged("AirFightPower");
        }
        
        protected override void OnAllPropertyChanged()
        {
            OnPropertyChanged("Name");
            OnPropertyChanged("MissionState");
            OnPropertyChanged("MissionID");
            OnPropertyChanged("MissionInfo");
            OnPropertyChanged("BackTime");
            OnPropertyChanged("BackTimeLocal");
            if (needupdateship) OnPropertyChanged("Ships");
        }
    }
    public enum FleetStatus { Ready, NotReady, InSortie, InMission, Warning }
}
