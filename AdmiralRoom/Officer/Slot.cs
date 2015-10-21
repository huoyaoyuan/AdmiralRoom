using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Slot : NotifyBase
    {

        #region IsLocked
        private bool _islocked;
        public bool IsLocked
        {
            get { return _islocked; }
            set
            {
                if (_islocked != value)
                {
                    _islocked = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
        
        #region Item
        private Equipment _item;
        public Equipment Item
        {
            get { return _item; }
            set
            {
                if (_item != value)
                {
                    _item = value;
                    OnAllPropertyChanged();
                }
            }
        }
        #endregion
        
        #region AirCraft
        private LimitedValue _ac;
        public LimitedValue AirCraft
        {
            get { return _ac; }
            set
            {
                _ac = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public bool HasItem => Item != null;
    }
}
