using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Huoyaoyuan.AdmiralRoom.Officer;
using Meowtrix.ComponentModel;

namespace Huoyaoyuan.AdmiralRoom.Models
{
    sealed class ShipCatalogWorker : NotificationObject, IUpdatable
    {
        public class ShipTypeSelector : NotifySourceObject<ShipCatalogWorker>
        {
            public ShipType ShipType { get; }
            public ShipTypeSelector(ShipType type, ShipCatalogWorker source) { ShipType = type; Source = source; }

            #region IsSelected
            private bool _isselected = true;
            public bool IsSelected
            {
                get { return _isselected; }
                set
                {
                    if (_isselected != value)
                    {
                        _isselected = value;
                        OnPropertyChanged();
                        Source.OnPropertyChanged(nameof(SelectAll));
                        Source.Update();
                    }
                }
            }
            #endregion
        }
        public class ShipFilter : NotifySourceObject<ShipCatalogWorker>
        {
            public string TextKey { get; set; }
            public string TitleKey => "Resources.Ship_Filter_" + TextKey + "_Title";
            public string TrueKey => "Resources.Ship_Filter_" + TextKey + "_True";
            public string FalseKey => "Resources.Ship_Filter_" + TextKey + "_False";
            public bool? Value { get; set; }
            public bool NoneSelected
            {
                get
                {
                    return Value == null;
                }
                set
                {
                    if (value)
                    {
                        Value = null;
                        OnAllPropertyChanged();
                    }
                }
            }
            public bool TrueSelected
            {
                get
                {
                    return Value == true;
                }
                set
                {
                    if (value)
                    {
                        Value = true;
                        OnAllPropertyChanged();
                    }
                }
            }
            public bool FalseSelected
            {
                get
                {
                    return Value == false;
                }
                set
                {
                    if (value)
                    {
                        Value = false;
                        OnAllPropertyChanged();
                    }
                }
            }
            public Func<Ship, bool> Filter { get; set; }
            public IEnumerable<Ship> Apply(IEnumerable<Ship> source) => !Value.HasValue ? source : Value.Value ? source.Where(Filter) : source.Where(x => !Filter(x));
        }
        public class ShipSortColumn
        {
            public string NameKey { get; set; } = "Sort_None";
            public string ResourceNameKey => "Resources.Ship_" + NameKey;
            public Func<Ship, int> KeySelector { get; set; } = x => 0;
            public bool IsDefaultDescend { get; set; }
        }
        public class ShipSortSelector : NotifySourceObject<ShipCatalogWorker>
        {
            public ShipSortColumn Sorter => Source.Sortings[SelectedIndex];
            public bool IsDescend => DescendBoxIndex == 1;

            #region SelectedIndex
            private int _selectedindex;
            public int SelectedIndex
            {
                get { return _selectedindex; }
                set
                {
                    if (_selectedindex != value)
                    {
                        if (Source?.Selectors != null)
                        {
                            bool islast = Source.Selectors.IndexOf(this) == Source.Selectors.Count - 1;
                            if (value == 0 & !islast)
                                Source.Selectors.Remove(this);
                            if (value != 0 && islast)
                                Source.Selectors.Add(new ShipSortSelector { Source = this.Source });
                        }
                        _selectedindex = value;
                        _descendboxindex = -1;
                        OnAllPropertyChanged();
                    }
                }
            }
            #endregion

            #region DescendBoxIndex
            private int _descendboxindex = -1;
            public int DescendBoxIndex
            {
                get
                {
                    if (_descendboxindex == -1)
                        _descendboxindex = Sorter.IsDefaultDescend ? 1 : 0;
                    return _descendboxindex;
                }
                set
                {
                    if (_descendboxindex != value)
                    {
                        _descendboxindex = value;
                        OnPropertyChanged();
                    }
                }
            }
            #endregion
        }
        private ShipCatalogWorker()
        {
            Filters.ForEach(x => x.Source = this);
            Selectors = new ObservableCollection<ShipSortSelector>
            {
                new ShipSortSelector { Source = this, SelectedIndex = 2 },
                new ShipSortSelector { Source = this }
            };
            ready = true;
        }
        public static ShipCatalogWorker Instance { get; } = new ShipCatalogWorker();

        #region ShownShips
        private IReadOnlyList<ItemWithIndex<Ship>> _shownships;
        public IReadOnlyList<ItemWithIndex<Ship>> ShownShips
        {
            get { return _shownships; }
            set
            {
                if (_shownships != value)
                {
                    _shownships = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region ShipTypes
        private IReadOnlyList<ShipTypeSelector> _shiptypes;
        public IReadOnlyList<ShipTypeSelector> ShipTypes
        {
            get { return _shiptypes; }
            set
            {
                if (_shiptypes != value)
                {
                    _shiptypes = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public bool? SelectAll
        {
            get
            {
#pragma warning disable CC0013
                if (ShipTypes.IsNullOrEmpty()) return null;
                else if (ShipTypes.All(x => x.IsSelected)) return true;
                else if (ShipTypes.All(x => !x.IsSelected)) return false;
                else return null;
#pragma warning restore CC0013
            }
            set
            {
                ready = false;
                ShipTypes.ForEach(x => x.IsSelected = value.Value);
                ready = true;
                Update();
                OnPropertyChanged();
            }
        }
        public ShipFilter[] Filters { get; } =
        {
            new ShipFilter { TextKey = "Level", Value = true, Filter = x => x.Level >= 2 },
            new ShipFilter { TextKey = "Lock", Value = true, Filter = x => x.IsLocked },
            new ShipFilter { TextKey = "Speed", Value = null, Filter = x => x.ShipInfo.Speed == ShipSpeed.High },
            new ShipFilter { TextKey = "Remodel", Value = null, Filter = x => !x.ShipInfo.CanUpgrade },
            new ShipFilter { TextKey = "Powerup", Value = null, Filter = x => x.IsMaxModernized },
            new ShipFilter { TextKey = "Mission", Value = null, Filter = x => x.InFleet != null && x.InFleet.MissionState != Fleet.FleetMissionState.None }
        };
        public ShipSortColumn[] Sortings { get; } =
        {
            new ShipSortColumn(),
            new ShipSortColumn { NameKey = "Id", KeySelector = x => x.Id, IsDefaultDescend = false },
            new ShipSortColumn { NameKey = "Level", KeySelector = x => x.Exp.Current, IsDefaultDescend = true },
            new ShipSortColumn { NameKey = "Class", KeySelector = x => x.ShipInfo.ShipType.SortNo, IsDefaultDescend = false },
            new ShipSortColumn { NameKey = "Name", KeySelector = x => x.ShipInfo.SortNo, IsDefaultDescend = false },
            new ShipSortColumn { NameKey = "Cond", KeySelector = x => x.Condition, IsDefaultDescend = true },
            new ShipSortColumn { NameKey = "Firepower", KeySelector = x => x.Firepower.Current, IsDefaultDescend = true },
            new ShipSortColumn { NameKey = "Torpedo", KeySelector = x => x.Torpedo.Current, IsDefaultDescend = true },
            new ShipSortColumn { NameKey = "AA", KeySelector = x => x.AA.Current, IsDefaultDescend = true },
            new ShipSortColumn { NameKey = "Armor", KeySelector = x => x.Armor.Current, IsDefaultDescend = true },
            new ShipSortColumn { NameKey = "Luck", KeySelector = x => x.Luck.Current, IsDefaultDescend = true },
            new ShipSortColumn { NameKey = "Evasion", KeySelector = x => x.Evasion.Current, IsDefaultDescend = true },
            new ShipSortColumn { NameKey = "ASW", KeySelector = x => x.ASW.Current, IsDefaultDescend = true },
            new ShipSortColumn { NameKey = "LoS", KeySelector = x => x.LoS.Current, IsDefaultDescend = true },
            new ShipSortColumn { NameKey = "RepairTime", KeySelector = x => (int)x.RepairTime.TotalSeconds, IsDefaultDescend = true }
        };
        public ObservableCollection<ShipSortSelector> Selectors { get; }
        public void Initialize()
        {
            ShipTypes = Staff.Current.MasterData.ShipTypes?.Select(x => new ShipTypeSelector(x, this)).ToArray() ?? new ShipTypeSelector[0];
        }
        private bool ready;
        public void Update()
        {
            if (!ready) return;
            IEnumerable<Ship> baseships = Staff.Current.Homeport.Ships;
            if (baseships == null) return;
            int[] typeid = ShipTypes.Where(x => x.IsSelected).Select(x => x.ShipType.Id).ToArray();
            baseships = baseships.Where(x => typeid.Contains(x.ShipInfo.ShipType.Id));
            Filters.ForEach(x => baseships = x.Apply(baseships));
            Ship[] sortedships = baseships.ToArray();
            MultiComparer<Ship> comparer = new MultiComparer<Ship> { Selectors = Selectors.TakeWhile(x => x.SelectedIndex != 0).Select(x => new Tuple<Func<Ship, int>, bool>(x.Sorter.KeySelector, x.IsDescend)) };
            Array.Sort(sortedships, comparer);
            ShownShips = sortedships.Select(ItemWithIndex<Ship>.Generator).ToArray();
        }
        public void SelectTypes(int[] types)
        {
            ready = false;
            foreach (var selector in ShipTypes)
            {
                selector.IsSelected = types.Contains(selector.ShipType.Id);
            }
            ready = true;
            Update();
        }
    }
}
