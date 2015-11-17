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
            ShipCatalogWorker Outer;
            public ShipType ShipType { get; private set; }
            public ShipTypeSelector(ShipType type, ShipCatalogWorker outer) { ShipType = type; Outer = outer; }

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
                        Outer.OnPropertyChanged("SelectAll");
                        Outer.Update();
                    }
                }
            }
            #endregion

        }
        public class ShipFilter
        {
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
                    if (value) Value = null;
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
                    if (value) Value = true;
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
                    if (value) Value = false;
                }
            }
            public Func<Ship, bool> Filter { get; set; }
            public IEnumerable<Ship> Apply(IEnumerable<Ship> source)
            {
                if (!Value.HasValue) return source;
                else return Value.Value ? source.Where(Filter) : source.Where(x => !Filter(x));
            }
        }
        private ShipCatalogWorker() { }
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
        public ShipFilter[] Filters { get; }
        public void Initialize()
        {
            ShipTypes = Staff.Current.MasterData.ShipTypes?.Select(x => new ShipTypeSelector(x, this)).ToArray() ?? new ShipTypeSelector[0];
        }
        public void Update()
        {
            IEnumerable<Ship> baseships = Staff.Current.Homeport.Ships;
            if (baseships == null) return;
            int[] typeid = ShipTypes.Select(x => x.ShipType.Id).ToArray();
            baseships = baseships.Where(x => typeid.Contains(x.ShipInfo.ShipType.Id));
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
