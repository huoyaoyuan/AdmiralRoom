using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huoyaoyuan.AdmiralRoom.Models
{
    class APIModel :NotifyBase
    {
        public static APIModel Current { get; } = new APIModel();
        #region APIText
        private string _apitext = "";
        public string APIText
        {
            get { return _apitext; }
            set
            {
                if (_apitext != value)
                {
                    _apitext = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}
