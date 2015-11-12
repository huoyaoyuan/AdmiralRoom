using System;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;

namespace Huoyaoyuan.AdmiralRoom
{
    [Serializable]
    public class Config : NotifyBase
    {
        public static Config Current { get; set; } = new Config();

        #region Language
        private string _language;
        public string Language
        {
            get { return _language; }
            set
            {
                if (_language != value)
                {
                    _language = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Theme
        private string _theme;
        public string Theme
        {
            get { return _theme; }
            set
            {
                if (value != _theme)
                {
                    _theme = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region NoDWM
        private bool _nodwm;
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
        private bool _aero;
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

        #region EnableProxy
        private bool _enableproxy;
        public bool EnableProxy
        {
            get { return _enableproxy; }
            set
            {
                if (value != _enableproxy)
                {
                    _enableproxy = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Proxy
        private Officer.Proxy _proxy;
        public Officer.Proxy Proxy
        {
            get { return _proxy; }
            set
            {
                if (value != _proxy)
                {
                    _proxy = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region EnableProxyHTTPS
        private bool _enalbeproxyhttps;
        public bool EnableProxyHTTPS
        {
            get { return _enalbeproxyhttps; }
            set
            {
                if (_enalbeproxyhttps != value)
                {
                    _enalbeproxyhttps = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region HTTPSProxy
        private Officer.Proxy _httpsproxy;
        public Officer.Proxy HTTPSProxy
        {
            get { return _httpsproxy; }
            set
            {
                if (_httpsproxy != value)
                {
                    _httpsproxy = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region ShowBuildingShipName
        private bool _showbuildingshipname;
        public bool ShowBuildingShipName
        {
            get { return _showbuildingshipname; }
            set
            {
                if (_showbuildingshipname != value)
                {
                    _showbuildingshipname = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public Config()
        {
            _theme = "Office 2013";
            var thisculture = CultureInfo.CurrentUICulture;
            foreach (var culture in ResourceService.SupportedCultures)
            {
                if (thisculture.ThreeLetterWindowsLanguageName == culture.ThreeLetterWindowsLanguageName)
                {
                    _language = culture.Name;
                    break;
                }
            }
            if (_language == null) _language = "en";
            var OSVersion = Environment.OSVersion.Version;
            if (OSVersion.Major >= 10)//Windows 10
            {
                _nodwm = true;
                _aero = false;
            }
            else if (OSVersion.Minor >= 2)//Windows 8
            {
                _nodwm = false;
                _aero = true;
            }
            else//Windows 7
            {
                _nodwm = false;
                _aero = false;
            }
            _proxy = new Officer.Proxy();
            _httpsproxy = new Officer.Proxy();
        }
        public static Config Load()
        {
            XmlSerializer s = new XmlSerializer(typeof(Config));
            try
            {
                using (var r = File.OpenText("config.xml"))
                {
                    return (Config)s.Deserialize(r);
                }
            }
            catch { return new Config(); }
        }
        public void Save()
        {
            XmlSerializer s = new XmlSerializer(typeof(Config));
            try
            {
                using (var w = File.CreateText("config.xml"))
                {
                    s.Serialize(w, this);
                }
            }
            catch { }
        }
    }
}
