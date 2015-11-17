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
        private ShipCatalogWorker() { }
        public static ShipCatalogWorker Instance { get; } = new ShipCatalogWorker();

        #region ShownShips
        private IEnumerable<Ship> _shownships;
        public IEnumerable<Ship> ShownShips
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

        public bool SelectAll
        {
            get
            {
                return ShipTypes?.All(x => x.IsSelected) ?? false;
            }
            set
            {
                ShipTypes.ForEach(x => x.IsSelected = value);
                OnPropertyChanged();
            }
        }
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
            ShownShips = baseships.ToArray();
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
