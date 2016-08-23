using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using Huoyaoyuan.AdmiralRoom.API;
using Huoyaoyuan.AdmiralRoom.Notifier;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Fleet : GameObject<getmember_deck>
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

        public Fleet(getmember_deck api) : base(api)
        {
            WeakEventManager<Timer, ElapsedEventArgs>.AddHandler(Staff.Current.Ticker, "Elapsed", Tick);
            WeakEventManager<Timer, ElapsedEventArgs>.AddHandler(Staff.Current.Ticker, "Elapsed", CheckHomeportRepairing);
            WeakEventManager<Config, PropertyChangedEventArgs>.AddHandler(Config.Current, nameof(PropertyChanged), (_, e) =>
            {
                if (e.PropertyName == nameof(Config.LosCalcType))
                    OnPropertyChanged(nameof(LoSInMap));
            });
        }
        private void Tick(object sender, ElapsedEventArgs e)
        {
            OnPropertyChanged(nameof(BackTime));
            OnPropertyChanged(nameof(ConditionTimeRemain));
            OnPropertyChanged(nameof(HomeportRepairingFrom));
            if (MissionState == FleetMissionState.InMission && Config.Current.NotifyWhenExpedition && BackTime.InASecond(Config.Current.NotifyTimeAdjust))
                NotifierFactories.Current?.Show(StringTable.Notification_Expedition_Title,
                    string.Format(StringTable.Notification_Expedition_Text, Name, MissionID, MissionInfo.Name),
                    Config.MakeSoundWithPath(Config.Current.NotifyExpeditionSound));
            if (!InSortie && Config.Current.NotifyWhenCondition && ConditionHelper.Instance.RemainCeiling(mincondition).InASecond())
                NotifierFactories.Current?.Show(StringTable.Notification_Condition_Title,
                    string.Format(StringTable.Notification_Condition_Text, Name),
                    Config.MakeSoundWithPath(Config.Current.NotifyConditionSound));
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

        #region HomeportRepaired
        private bool _homeportrepaired;
        public bool HomeportRepaired
        {
            get { return _homeportrepaired; }
            set
            {
                if (_homeportrepaired != value)
                {
                    _homeportrepaired = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        private bool needupdateship;
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
        public int LevelSum => Ships.Sum(x => x.Level);
        public double LevelAverage => Ships.Any() ? Ships.Average(x => (double)x.Level) : 0;
        public double LoSInMap
        {
            get
            {
                switch (Config.Current.LosCalcType)
                {
                    case LosCalcType.SimpleSum:
                    case LosCalcType.Formula14Q3:
                        return Ships.Sum(x => x.LoSInMap);
                    case LosCalcType.Formula14Q4:
                        return Ships.Sum(x => x.LoSInMap) - Math.Ceiling(Staff.Current.Admiral.Level / 5.0) * 5.0 * 0.61;
                    case LosCalcType.Formula16Q1:
                        return Ships.Sum(x => x.LoSInMap) - Math.Ceiling(Staff.Current.Admiral.Level * 0.4) + (6 - Ships.Count) * 2;
                    default:
                        return 0;
                }
            }
        }
        public int[] ChargeCost => new[]
        {
            Ships.Sum(x => x.ApplyMarriage(x.Fuel.Shortage)),
            Ships.Sum(x => x.ApplyMarriage(x.Bull.Shortage)),
            Ships.Sum(x => x.Slots.Sum(y => y.AirCraft.Shortage)) * 5
        };
        public int[] RepairCost => new[] { Ships.Sum(x => x.RepairFuel), Ships.Sum(x => x.RepairSteel) };
        private int mincondition = 49;
        public void UpdateStatus()
        {
            LowCondition = false;
            HeavilyDamaged = false;
            NeedCharge = false;
            Repairing = false;
            foreach (var ship in Ships)
            {
                LowCondition |= ship.Condition < 40;
                HeavilyDamaged |= !ship.IsEscaped && ship.DamageControl == null && ship != Ships.FirstOrDefault() && ship.HP.Current * 4 <= ship.HP.Max;
                NeedCharge |= !(ship.Fuel.IsMax && ship.Bull.IsMax);
                Repairing |= ship.IsRepairing;
            }
#pragma warning disable CC0014
            if (InSortie)
                Status = HeavilyDamaged ? FleetStatus.Warning : FleetStatus.InSortie;
            else if (MissionState != FleetMissionState.None)
                Status = FleetStatus.InMission;
            else if (NeedCharge || HeavilyDamaged || LowCondition || Repairing || CanHomeportRepairing)
                Status = FleetStatus.NotReady;
            else Status = FleetStatus.Ready;
#pragma warning restore CC0014
            AirFightPower = Ships.Aggregate(new int[8], (x, y) => x.Zip(y.AirFightPower, (a, b) => a + b).ToArray());
            if (Ships.Any())
                mincondition = Ships.Min(x => x.Condition);
            else
            {
                mincondition = 49;
                Status = FleetStatus.Empty;
            }
            OnPropertyChanged(nameof(AirFightPower));
            OnPropertyChanged(nameof(LevelSum));
            OnPropertyChanged(nameof(LevelAverage));
            OnPropertyChanged(nameof(LoSInMap));
            OnPropertyChanged(nameof(ChargeCost));
            OnPropertyChanged(nameof(RepairCost));
            OnPropertyChanged(nameof(CanHomeportRepairing));
        }

        protected override void OnAllPropertyChanged()
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(MissionState));
            OnPropertyChanged(nameof(MissionID));
            OnPropertyChanged(nameof(MissionInfo));
            OnPropertyChanged(nameof(BackTime));
            OnPropertyChanged(nameof(ConditionTimeRemain));
            OnPropertyChanged(nameof(ConditionTimeOffset));
            if (needupdateship) OnPropertyChanged(nameof(Ships));
        }

#pragma warning disable CC0049
        public bool CanHomeportRepairing
            => Ships.Count > 0
            && Ships[0].ShipInfo.ShipType.Id == 19
            && Ships[0].IsRepairing == false
            && Ships[0].HP.Percentage > 0.5
            && MissionState == FleetMissionState.None;
#pragma warning restore CC0049
        private IEnumerable<Ship> HomeportRepairingList => Ships.Take(
            CanHomeportRepairing ? Ships[0].Slots.Count(x => x.Item?.EquipInfo.EquipType.Id == 31) + 2 : 0)
            .Where(x => !x.HP.IsMax && x.HP.Current * 2 > x.HP.Max);

        public DateTimeOffset HomeportRepairingFrom { get; private set; } = DateTimeOffset.UtcNow;
        public void CheckHomeportRepairingTime(bool reset)
        {
            var time = DateTimeOffset.UtcNow;
            if (reset || (time - HomeportRepairingFrom).TotalMinutes >= 20) HomeportRepairingFrom = time;
            CheckHomeportRepairing(null, null);
        }

        private void CheckHomeportRepairing(object sender, ElapsedEventArgs e)
        {
            var during = DateTimeOffset.UtcNow - HomeportRepairingFrom;
            HomeportRepaired = during.TotalMinutes >= 20;
            foreach (var ship in HomeportRepairingList)
                if (!ship.IsRepairing)
                {
                    if (HomeportRepaired)
                        ship.RepairingHP = ship.HP.Current + Math.Max((int)((during.TotalSeconds - 60) / ship.RepairTimePerHP.TotalSeconds), 1);
                    if (ship.RepairingHP > ship.HP.Max)
                        ship.RepairingHP = ship.HP.Max;
                    if (!HomeportRepaired)
                        ship.NextHP = HomeportRepairingFrom.AddMinutes(20);
                    else if (ship.RepairingHP != ship.HP.Max)
                        ship.NextHP = HomeportRepairingFrom.AddSeconds((ship.RepairedHP + 1) * ship.RepairTimePerHP.TotalSeconds + 60);
                }
        }

        public void LosTypeChanged() => OnPropertyChanged(nameof(LoSInMap));
    }
    public enum FleetStatus { Empty, Ready, NotReady, InSortie, InMission, Warning }
}
