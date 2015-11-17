using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Huoyaoyuan.AdmiralRoom.Officer;

namespace Huoyaoyuan.AdmiralRoom.Models
{
    class ShipCatalogWorker : NotificationObject
    {
        private ShipCatalogWorker() { }
        public static ShipCatalogWorker Instance { get; } = new ShipCatalogWorker();

        #region ShownShips
        private IReadOnlyCollection<Ship> _shownships;
        public IReadOnlyCollection<Ship> ShownShips
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

        public void Update()
        {
            ShownShips = Staff.Current.Homeport.Ships?.ToArray();
        }
    }
}
