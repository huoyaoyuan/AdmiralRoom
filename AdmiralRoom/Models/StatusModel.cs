using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huoyaoyuan.AdmiralRoom.Models
{
    class StatusModel : NotifyBase
    {
        public static StatusModel Current { get; } = new StatusModel();
        #region StatusText
        private string _statustext = "";
        public string StatusText
        {
            get { return _statustext; }
            set
            {
                if(_statustext != value)
                {
                    _statustext = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}
