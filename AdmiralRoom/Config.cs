using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace Huoyaoyuan.AdmiralRoom
{
    [Serializable]
    public class Config : NotifyBase
    {
        public static Config Current { get; set; } = new Config();
        #region Theme
        private int _theme = 2;
        public int Theme
        {
            get { return _theme; }
            set
            {
                if(value!= _theme)
                {
                    _theme = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
        #region NoDWM
        private bool _nodwm = false;
        public bool NoDWM
        {
            get { return _nodwm; }
            set
            {
                if (value != _nodwm)
                {
                    _nodwm = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
        #region Aero
        private bool _aero = true;
        public bool Aero
        {
            get { return _aero; }
            set
            {
                if (value != _aero)
                {
                    _aero = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
        public static Config Load()
        {
            XmlSerializer s = new XmlSerializer(typeof(Config));
            Config c = new Config();
            try
            {
                using (var r = File.OpenText("config.xml"))
                {
                    c = (Config)s.Deserialize(r);
                }
            }
            catch { }
            return c;
        }
        public void Save()
        {
            XmlSerializer s = new XmlSerializer(typeof(Config));
            try
            {
                using (var fs = File.Open("config.xml", FileMode.Truncate))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        s.Serialize(sw, this);
                    }
                }
            }
            catch { }
        }
    }
}
