using System;
using System.Collections.Generic;
using System.Linq;
using Huoyaoyuan.AdmiralRoom.Officer;

namespace Huoyaoyuan.AdmiralRoom.Models
{
    class ShipCatalogWorker : NotificationObject
    {
        public class ShipTypeSelector : NotificationObject
        {
            ShipCatalogWorker Source;
            public ShipType ShipType { get; private set; }
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
        public class ShipFilter : NotificationObject
        {
            public ShipCatalogWorker Source { get; set; }
            public string Title { get; set; }
            public string TrueText { get; set; }
            public string FalseText { get; set; }
            public bool? Value { get; set; }
            public bool NoneSelected
            {
                get
                {
                    return !Value.HasValue;
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
                    return Value.HasValue && Value.Value;
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
                    return Value.HasValue && !Value.Value;
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
            protected override void OnAllPropertyChanged()
            {
                base.OnAllPropertyChanged();
                Source.Update();
            }
        }
        private ShipCatalogWorker() { Filters.ForEach(x => x.Source = this); }
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
                ShipTypes.ForEach(x => x.IsSelected = value.Value);
                OnPropertyChanged();
            }
        }
        public ShipFilter[] Filters { get; } =
        {
            new ShipFilter { Title = "等级", TrueText = "Lv.2 以上", FalseText = "Lv.1", Value = true, Filter = x => x.Level >= 2 },
            new ShipFilter { Title = "锁定", TrueText = "已锁", FalseText = "未锁", Value = true, Filter = x => x.IsLocked },
            new ShipFilter { Title = "速度", TrueText = "高速", FalseText = "低速", Value = null, Filter = x => x.ShipInfo.Speed == ShipSpeed.High },
            new ShipFilter { Title = "改造", TrueText = "改造完毕", FalseText = "改造未完", Value = null, Filter = x => !x.ShipInfo.CanUpgrade },
            new ShipFilter { Title = "近代化改修", TrueText = "改修完毕", FalseText = "改修未完", Value = null, Filter = x => x.IsMaxModernized }
        };
        public void Initialize()
        {
            ShipTypes = Staff.Current.MasterData.ShipTypes?.Select(x => new ShipTypeSelector(x, this)).ToArray() ?? new ShipTypeSelector[0];
        }
        public void Update()
        {
            IEnumerable<Ship> baseships = Staff.Current.Homeport.Ships;
            if (baseships == null) return;
            int[] typeid = ShipTypes.Where(x => x.IsSelected).Select(x => x.ShipType.Id).ToArray();
            baseships = baseships.Where(x => typeid.Contains(x.ShipInfo.ShipType.Id));
            Filters.ForEach(x => baseships = x.Apply(baseships));
            ShownShips = baseships.Select(ItemWithIndex<Ship>.Generator).ToArray();
        }
        public void SelectTypes(int[] types)
        {
            foreach (var selector in ShipTypes)
            {
                selector.IsSelected = types.Contains(selector.ShipType.Id);
            }
        }
    }
}
