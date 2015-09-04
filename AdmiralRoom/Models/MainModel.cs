using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huoyaoyuan.AdmiralRoom.Models
{
    class MainModel : NotifyBase
    {
        public static MainModel Current { get; } = new MainModel();
        #region TestText
        private string _testtext = "";
        public string TestText
        {
            get { return _testtext; }
            set
            {
                if (_testtext != value)
                {
                    _testtext = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}
