using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Ship : GameObject<api_ship>
    {
        public Ship() { }
        public Ship(api_ship api) : base(api) { }
        public override int Id => rawdata.api_id;
        public int SortNo => rawdata.api_sortno;
        public int ShipId => rawdata.api_ship_id;
        public int Level => rawdata.api_lv;
        public Exp Exp { get; private set; }
        public LimitedValue HP { get; private set; }
        public ShootRange Range => (ShootRange)rawdata.api_leng;

        #region Slots
        private ObservableCollection<Slot> _slots;
        public ObservableCollection<Slot> Slots
        {
            get { return _slots; }
            set
            {
                if (_slots != value)
                {
                    _slots = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public Slot SlotEx { get; private set; }
        public Modernizable Firepower { get; private set; }
        public Modernizable Torpedo { get; private set; }
        public Modernizable AA { get; private set; }
        public Modernizable Armor { get; private set; }
        public bool IsMaxModernized => Firepower.IsMax && Torpedo.IsMax && AA.IsMax && Armor.IsMax;
        public Modernizable Luck { get; private set; }
        public LimitedValue Evasion { get; private set; }
        public LimitedValue ASW { get; private set; }
        public LimitedValue LoS { get; private set; }
        public int Rare => rawdata.api_backs;

        #region Fuel
        private LimitedValue _fuel;
        public LimitedValue Fuel
        {
            get { return _fuel; }
            set
            {
                _fuel = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Bull
        private LimitedValue _bull;
        public LimitedValue Bull
        {
            get { return _bull; }
            set
            {
                _bull = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public int SlotNum => rawdata.api_slotnum;
        public TimeSpan RepairTime => TimeSpan.FromMilliseconds(rawdata.api_ndock_time);
        public int RepairFuel => rawdata.api_ndock_item[0];
        public int RepairSteel => rawdata.api_ndock_item[1];
        public TimeSpan RepairTimePerHP { get; private set; }
        public int MordenizeRate => rawdata.api_srate;
        public int Condition { get; private set; } = 49;
        public bool IsLocked => rawdata.api_locked != 0;
        public bool LockedEquip => rawdata.api_locked_equip != 0;
        public ShipInfo ShipInfo => Staff.Current.MasterData.ShipInfo[ShipId];

        #region IsRepairing
        private bool _isrepairing;
        public bool IsRepairing
        {
            get { return _isrepairing; }
            set
            {
                if (_isrepairing != value)
                {
                    _isrepairing = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region RepairingHP
        private int _repairingHP;
        public int RepairingHP
        {
            get { return _repairingHP; }
            set
            {
                if (_repairingHP != value)
                {
                    _repairingHP = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public Fleet InFleet { get; set; }

        private bool _ignorenextcondition = true;
        public void IgnoreNextCondition() => _ignorenextcondition = true;
        private bool _hpchanged = false;
        protected override void UpdateProp()
        {
            Exp = new Exp(rawdata.api_exp, Level, true);
            _hpchanged = HP.Current != rawdata.api_nowhp;
            HP = new LimitedValue(rawdata.api_nowhp, rawdata.api_maxhp);
            if (!IsRepairing) RepairingHP = HP.Current;
            Evasion = new LimitedValue(rawdata.api_kaihi);
            int asw = rawdata.api_taisen[0];
            int los = rawdata.api_sakuteki[0];
            //ASW = new LimitedValue(rawdata.api_taisen);
            //LoS = new LimitedValue(rawdata.api_sakuteki);
            if (_ignorenextcondition) _ignorenextcondition = false;
            else if (rawdata.api_cond <= 49)
                ConditionHelper.Instance.OnCondition(rawdata.api_cond - Condition);
            Condition = rawdata.api_cond;
            Firepower = new Modernizable(ShipInfo.FirePower, rawdata.api_kyouka[0], rawdata.api_karyoku[0]);
            Torpedo = new Modernizable(ShipInfo.Torpedo, rawdata.api_kyouka[1], rawdata.api_raisou[0]);
            AA = new Modernizable(ShipInfo.AA, rawdata.api_kyouka[2], rawdata.api_taiku[0]);
            Armor = new Modernizable(ShipInfo.Armor, rawdata.api_kyouka[3], rawdata.api_soukou[0]);
            Luck = new Modernizable(ShipInfo.Luck, rawdata.api_kyouka[4], rawdata.api_lucky[0]);
            Fuel = new LimitedValue(rawdata.api_fuel, ShipInfo.MaxFuel);
            Bull = new LimitedValue(rawdata.api_bull, ShipInfo.MaxBull);
            Slots?.ForEach(x => x.Item?.SetNotOnShip());
            var slots = new List<Slot>();
            for (int i = 0; i < SlotNum; i++)
            {
                var slot = new Slot();
                if (rawdata.api_slot[i] != -1 && (slot.Item = Staff.Current.Homeport.Equipments[rawdata.api_slot[i]]) != null)
                {
                    slot.Item.OnShip = this;
                    asw -= slot.Item.EquipInfo.ASW;
                    los -= slot.Item.EquipInfo.LoS;
                }
                slots.Add(slot);
            }
            ASW = new LimitedValue(asw, rawdata.api_taisen[1]);
            LoS = new LimitedValue(los, rawdata.api_sakuteki[1]);
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].AirCraft = new LimitedValue(rawdata.api_onslot[i], ShipInfo.AirCraft[i]);
            }
            Slots = new ObservableCollection<Slot>(slots);
            SlotEx?.Item?.SetNotOnShip();
            SlotEx = new Slot();
            if (rawdata.api_slot_ex == 0) SlotEx.IsLocked = true;
            else if (rawdata.api_slot_ex != -1)
            {
                SlotEx.Item = Staff.Current.Homeport.Equipments[rawdata.api_slot_ex];
                SlotEx.Item.OnShip = this;
            }
            RepairTimePerHP = HP.IsMax ? TimeSpan.Zero : TimeSpan.FromSeconds((RepairTime.TotalSeconds - 30) / HP.Shortage);
            UpdateStatus();
        }

        public bool FindEquipment(int itemid) => rawdata.api_slot.Contains(itemid) || (SlotEx.Item?.Id == itemid);
        public void SetRepaired()
        {
            RepairingHP = HP.Max;
            rawdata.api_ndock_time = 0;
            rawdata.api_ndock_item = new[] { 0, 0 };
            IsRepairing = false;
            _ignorenextcondition = true;
        }
        public int ApplyMarriage(int raw) => Level >= 100 ? (int)(raw * 0.85) : raw;
        public int[] AirFightPower => Slots.Aggregate(new int[8], (x, y) => x.Zip(y.AirFightPower, (a, b) => a + (int)b).ToArray());
        public double LoSInMap => Slots.Select(x => x.LoSInMap).Sum() + Math.Sqrt(LoS.Current) * 1.69;
        public void UpdateStatus()
        {
            OnPropertyChanged(nameof(AirFightPower));
            OnPropertyChanged(nameof(LoSInMap));
            if (InFleet != null && InFleet.Ships.IndexOf(this) == -1) InFleet = null;
            if (_hpchanged) InFleet?.CheckHomeportRepairingTime(true);
            InFleet?.UpdateStatus();
        }
    }
    public enum ShootRange { None = 0, Short = 1, Long = 2, VLong = 3 }
}
