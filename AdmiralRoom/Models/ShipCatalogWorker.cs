using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Huoyaoyuan.AdmiralRoom.Officer;

namespace Huoyaoyuan.AdmiralRoom.Models
{
    class ShipCatalogWorker : NotificationObject, IUpdatable
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
                        Source.OnPropertyChanged("SelectAll");
                        Source.Update();
                    }
                }
            }
            #endregion
        }
        public class ShipFilter : NotifySourceObject<ShipCatalogWorker>
        {
            public string Title { get; set; }
            public string TrueText { get; set; }
            public string FalseText { get; set; }
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
            public IEnumerable<Ship> Apply(IEnumerable<Ship> source)
            {
                if (!Value.HasValue) return source;
                else return Value.Value ? source.Where(Filter) : source.Where(x => !Filter(x));
            }
        }
        public class ShipSortColumn
        {
            public string Name { get; set; } = "（无）";
            public Func<Ship, int> KeySelector { get; set; } = x => 0;
            public bool IsDefaultDescend { get; set; }
            public override string ToString() => Name;
        }
        public class ShipSortSelector : NotifySourceObject<ShipCatalogWorker>
        {
            public ShipSortColumn Sorter => Source.Sortings[SelectedIndex];
            public bool IsDescend => DescendBoxIndex == 1;

            #region SelectedIndex
            private int _selectedindex = 0;
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
        private IEnumerable<ItemWithIndex<Ship>> _shownships;
        public IEnumerable<ItemWithIndex<Ship>> ShownShips
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
        private IEnumerable<ShipTypeSelector> _shiptypes;
        public IEnumerable<ShipTypeSelector> ShipTypes
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
                if (ShipTypes.IsNullOrEmpty()) return null;
                else if (ShipTypes.All(x => x.IsSelected)) return true;
                else if (ShipTypes.All(x => !x.IsSelected)) return false;
                else return null;
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
            new ShipFilter { Title = "等级", TrueText = "Lv.2 以上", FalseText = "Lv.1", Value = true, Filter = x => x.Level >= 2 },
            new ShipFilter { Title = "锁定", TrueText = "已锁", FalseText = "未锁", Value = true, Filter = x => x.IsLocked },
            new ShipFilter { Title = "速度", TrueText = "高速", FalseText = "低速", Value = null, Filter = x => x.ShipInfo.Speed == ShipSpeed.High },
            new ShipFilter { Title = "改造", TrueText = "改造完毕", FalseText = "改造未完", Value = null, Filter = x => !x.ShipInfo.CanUpgrade },
            new ShipFilter { Title = "近代化改修", TrueText = "改修完毕", FalseText = "改修未完", Value = null, Filter = x => x.IsMaxModernized },
            new ShipFilter { Title = "远征", TrueText = "远征中", FalseText = "不在远征中", Value = null, Filter = x => x.InFleet != null && x.InFleet.MissionState != Fleet.FleetMissionState.None }
        };
        public ShipSortColumn[] Sortings { get; } =
        {
            new ShipSortColumn(),
            new ShipSortColumn { Name = "Id", KeySelector = x => x.Id, IsDefaultDescend = false },
            new ShipSortColumn { Name = "等级与经验", KeySelector = x => x.Exp.Current, IsDefaultDescend = true },
            new ShipSortColumn { Name = "舰种", KeySelector = x => x.ShipInfo.ShipType.SortNo, IsDefaultDescend = false },
            new ShipSortColumn { Name = "舰名", KeySelector = x => x.ShipInfo.SortNo, IsDefaultDescend = false },
            new ShipSortColumn { Name = "疲劳", KeySelector = x => x.Condition, IsDefaultDescend = true },
            new ShipSortColumn { Name = "火力", KeySelector = x => x.Firepower.Current, IsDefaultDescend = true },
            new ShipSortColumn { Name = "雷装", KeySelector = x => x.Torpedo.Current, IsDefaultDescend = true },
            new ShipSortColumn { Name = "对空", KeySelector = x => x.AA.Current, IsDefaultDescend = true },
            new ShipSortColumn { Name = "装甲", KeySelector = x => x.Armor.Current, IsDefaultDescend = true },
            new ShipSortColumn { Name = "运", KeySelector = x => x.Luck.Current, IsDefaultDescend = true },
            new ShipSortColumn { Name = "回避", KeySelector = x => x.Evasion.Current, IsDefaultDescend = true },
            new ShipSortColumn { Name = "对潜", KeySelector = x => x.ASW.Current, IsDefaultDescend = true },
            new ShipSortColumn { Name = "索敌", KeySelector = x => x.LoS.Current, IsDefaultDescend = true },
            new ShipSortColumn { Name = "修理时间", KeySelector = x => (int)x.RepairTime.TotalSeconds, IsDefaultDescend = true }
        };
        public ObservableCollection<ShipSortSelector> Selectors { get; }
        public void Initialize()
        {
            ShipTypes = Staff.Current.MasterData.ShipTypes?.Select(x => new ShipTypeSelector(x, this)).ToArray() ?? new ShipTypeSelector[0];
        }
        private bool ready = false;
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
