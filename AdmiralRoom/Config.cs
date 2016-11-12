using System;
using System.Globalization;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Huoyaoyuan.AdmiralRoom.Notifier;
using Meowtrix.ComponentModel;
using Meowtrix.Globalization;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Huoyaoyuan.AdmiralRoom
{
    public sealed class Config : NotificationObject
    {
        static Config()
        {
            ResourceService.CultureChanged += (_, e) => Current._language = e.NewValue.Name;
        }
        public static Config Current { get; } = new Config();

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
                    ResourceService.ChangeCulture(value);
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
                if (_theme != value)
                {
                    _theme = value;
                    OnPropertyChanged();
                    ThemeService.CurrentTheme = value;
                }
            }
        }
        #endregion

        #region SystemTheme
        private string _systemtheme;
        public string SystemTheme
        {
            get { return _systemtheme; }
            set
            {
                if (_systemtheme != value)
                {
                    _systemtheme = value;
                    OnPropertyChanged();
                    ThemeService.SetSystemTheme(value);
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

        #region OverrideGameUrl
        private string _overridegameurl;
        public string OverrideGameUrl
        {
            get { return _overridegameurl; }
            set
            {
                if (_overridegameurl != value)
                {
                    _overridegameurl = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(OverrideGameUrlEnabled));
                }
            }
        }
        [XmlIgnore]
        [MemberwiseCopyIgnore]
        public bool OverrideGameUrlEnabled
        {
            get { return OverrideGameUrl != null; }
            set { OverrideGameUrl = value ? string.Empty : null; }
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

        #region NotifierType
        public NotifierType NotifierType
        {
            get { return NotifierFactories.CurrentType; }
            set
            {
                NotifierFactories.CurrentType = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region BrowserZoomFactor
        private double _browserzoomfactor;
        public double BrowserZoomFactor
        {
            get { return _browserzoomfactor; }
            set
            {
                if (_browserzoomfactor != value)
                {
                    _browserzoomfactor = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region ScreenShotFolder
        private string _screenshotfolder;
        public string ScreenShotFolder
        {
            get { return _screenshotfolder; }
            set
            {
                if (_screenshotfolder != value)
                {
                    _screenshotfolder = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region ScreenShotNameFormat
        private string _screenshotnameformat;
        public string ScreenShotNameFormat
        {
            get { return _screenshotnameformat; }
            set
            {
                if (_screenshotnameformat != value)
                {
                    if (!value.Contains("{0}")) value = value + "{0}";
                    _screenshotnameformat = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region ScreenShotFileFormat
        private string _screenshotfileformat;
        public string ScreenShotFileFormat
        {
            get { return _screenshotfileformat; }
            set
            {
                if (_screenshotfileformat != value)
                {
                    _screenshotfileformat = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region NotifyWhenExpedition
        private bool _notifywhenexpedition;
        public bool NotifyWhenExpedition
        {
            get { return _notifywhenexpedition; }
            set
            {
                if (_notifywhenexpedition != value)
                {
                    _notifywhenexpedition = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region NotifyWhenRepair
        private bool _notifywhenrepair;
        public bool NotifyWhenRepair
        {
            get { return _notifywhenrepair; }
            set
            {
                if (_notifywhenrepair != value)
                {
                    _notifywhenrepair = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region NotifyWhenBuild
        private bool _notifywhenbuild;
        public bool NotifyWhenBuild
        {
            get { return _notifywhenbuild; }
            set
            {
                if (_notifywhenbuild != value)
                {
                    _notifywhenbuild = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region NotifyWhenCondition
        private bool _notifywhencondition;
        public bool NotifyWhenCondition
        {
            get { return _notifywhencondition; }
            set
            {
                if (_notifywhencondition != value)
                {
                    _notifywhencondition = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region NotifyTimeAdjust
        private int _notifytimeadjust;
        public int NotifyTimeAdjust
        {
            get { return _notifytimeadjust; }
            set
            {
                if (_notifytimeadjust != value)
                {
                    _notifytimeadjust = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region FontFamily
        private FontFamily _fontfamily;
        [XmlIgnore]
        public FontFamily FontFamily
        {
            get { return _fontfamily; }
            set
            {
                if (_fontfamily != value)
                {
                    _fontfamily = value;
                    OnPropertyChanged();
                }
            }
        }
        [MemberwiseCopyIgnore]
        public string FontFamilyName
        {
            get { return string.Join(",", FontFamily.FamilyNames.Values); }
            set { FontFamily = new FontFamily(value); }
        }
        #endregion

        #region FontSize
        private double _fontsize;
        public double FontSize
        {
            get { return _fontsize; }
            set
            {
                if (_fontsize != value)
                {
                    _fontsize = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region NotifyExpeditionSound
        private string _notifyexpeditionsound = "expedition.mp3";
        public string NotifyExpeditionSound
        {
            get { return _notifyexpeditionsound; }
            set
            {
                if (_notifyexpeditionsound != value)
                {
                    _notifyexpeditionsound = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region NotifyConditionSound
        private string _notifyconditionsound = "condition.mp3";
        public string NotifyConditionSound
        {
            get { return _notifyconditionsound; }
            set
            {
                if (_notifyconditionsound != value)
                {
                    _notifyconditionsound = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region NotifyBuildSound
        private string _notifybuildsound = "build.mp3";
        public string NotifyBuildSound
        {
            get { return _notifybuildsound; }
            set
            {
                if (_notifybuildsound != value)
                {
                    _notifybuildsound = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region NotifyRepairSound
        private string _notifyrepairsound = "repair.mp3";
        public string NotifyRepairSound
        {
            get { return _notifyrepairsound; }
            set
            {
                if (_notifyrepairsound != value)
                {
                    _notifyrepairsound = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region LosCalcType
        private LosCalcType _loscalctype;
        public LosCalcType LosCalcType
        {
            get { return _loscalctype; }
            set
            {
                if (_loscalctype != value)
                {
                    _loscalctype = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region ReportToPoiDB
        private bool _reporttopoidb;
        public bool ReportToPoiDB
        {
            get { return _reporttopoidb; }
            set
            {
                if (_reporttopoidb != value)
                {
                    _reporttopoidb = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region ReportToKancolleDB
        private bool _reporttokancolledb;
        public bool ReportToKancolleDB
        {
            get { return _reporttokancolledb; }
            set
            {
                if (_reporttokancolledb != value)
                {
                    _reporttokancolledb = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region KancolleDBToken
        private string _kancolledbtoken;
        public string KancolleDBToken
        {
            get { return _kancolledbtoken; }
            set
            {
                if (_kancolledbtoken != value)
                {
                    _kancolledbtoken = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region AutoCheckUpdate
        private bool _autocheckupdate;
        public bool AutoCheckUpdate
        {
            get { return _autocheckupdate; }
            set
            {
                if (_autocheckupdate != value)
                {
                    _autocheckupdate = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region AutoDownloadUpdate
        private bool _autodownloadupdate;
        public bool AutoDownloadUpdate
        {
            get { return _autodownloadupdate; }
            set
            {
                if (_autodownloadupdate != value)
                {
                    _autodownloadupdate = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region UpdateUsingProxy
        private bool _updateusingproxy;
        public bool UpdateUsingProxy
        {
            get { return _updateusingproxy; }
            set
            {
                if (_updateusingproxy != value)
                {
                    _updateusingproxy = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region ShowMainBackground
        private bool _showmainwindowbackground;
        public bool ShowMainBackground
        {
            get { return _showmainwindowbackground; }
            set
            {
                if (_showmainwindowbackground != value)
                {
                    _showmainwindowbackground = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region MainBackgroundImage
        private string _mainbackgroundimage;
        public string MainBackgroundImage
        {
            get { return _mainbackgroundimage; }
            set
            {
                if (_mainbackgroundimage != value)
                {
                    _mainbackgroundimage = value;
                    OnPropertyChanged();
                    try
                    {
                        MainBackgroundImageSource = new BitmapImage(new Uri(value));
                        MainBackgroundImageSource.Freeze();
                    }
                    catch { MainBackgroundImageSource = null; }
                    OnPropertyChanged(nameof(MainBackgroundImageSource));
                }
            }
        }
        [XmlIgnore, MemberwiseCopyIgnore]
        public ImageSource MainBackgroundImageSource { get; private set; }
        #endregion

        #region MainBackgroundOpacity
        private double _mainbackgroundopacity;
        public double MainBackgroundOpacity
        {
            get { return _mainbackgroundopacity; }
            set
            {
                if (_mainbackgroundopacity != value)
                {
                    _mainbackgroundopacity = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region MainBackgroundBlurRadius
        private double _mainbackgroundblurradius;
        public double MainBackgroundBlurRadius
        {
            get { return _mainbackgroundblurradius; }
            set
            {
                if (_mainbackgroundblurradius != value)
                {
                    _mainbackgroundblurradius = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public static string MakeSoundWithPath(string filename) => Path.Combine("sound", filename);

        private Config()
        {
            var thisculture = CultureInfo.CurrentUICulture;
            foreach (var culture in ResourceService.SupportedCultures)
                if (culture.IsAncestorOf(thisculture))
                {
                    Language = culture.Name;
                    break;
                }
            if (Language == null) Language = "en";
            var OSVersion = Environment.OSVersion.Version;
            SystemTheme = OSVersion.Major == 6 && OSVersion.Minor == 1 ? "Aero" : "Aero2";
            Theme = "Default";
            _proxy = new Officer.Proxy();
            _httpsproxy = new Officer.Proxy();
            NotifierType = NotifierFactories.DefaultType;
            _browserzoomfactor = 1;
            _screenshotfolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            _screenshotnameformat = "KanColle-{0}";
            _screenshotfileformat = "png";
            _notifytimeadjust = 60;
            FontFamilyName = "DengXian";
            _fontsize = 15;
            _loscalctype = LosCalcType.Formula16Q1;
            _autocheckupdate = true;
            _autodownloadupdate = false;
            _updateusingproxy = true;
            _mainbackgroundopacity = 1;
        }
        public static Config Load(string path = "config.xml")
        {
            XmlSerializer s = new XmlSerializer(typeof(Config));
            try
            {
                using (var r = File.OpenText(path))
                {
                    return (Config)s.Deserialize(r);
                }
            }
            catch { return new Config(); }
        }
        public void Save(string path = "config.xml")
        {
            XmlSerializer s = new XmlSerializer(typeof(Config));
            try
            {
                using (var w = File.CreateText(path))
                {
                    s.Serialize(w, this);
                }
            }
            catch { }
        }
        public string GenerateScreenShotFileName() =>
            Path.Combine(ScreenShotFolder, string.Format(ScreenShotNameFormat, DateTime.Now.ToString("yyMMdd-HHmmssff")) + "." + ScreenShotFileFormat.ToLower());
        public Officer.Proxy GetHTTPSProxy() => EnableProxyHTTPS ? HTTPSProxy : EnableProxy ? Proxy : null;
        public class CommandSet
        {
            public ICommand Save { get; set; }
            public ICommand Load { get; set; }
            public ICommand SaveAs { get; set; }
            public ICommand LoadFrom { get; set; }
        }
        public static CommandSet Commands { get; } = new CommandSet
        {
            Save = new DelegateCommand(() => Current.Save()),
            Load = new DelegateCommand(() => Current.MemberwiseCopy(Load())),
            SaveAs = new DelegateCommand(() =>
            {
                using (var filedialog = new CommonSaveFileDialog())
                {
                    filedialog.InitialDirectory = Environment.CurrentDirectory;
                    filedialog.DefaultFileName = "config.xml";
                    filedialog.Filters.Add(new CommonFileDialogFilter("Xml Files", "*.xml"));
                    filedialog.Filters.Add(new CommonFileDialogFilter("All Files", "*"));
                    if (filedialog.ShowDialog() == CommonFileDialogResult.Ok)
                        Current.Save(filedialog.FileName);
                }
            }),
            LoadFrom = new DelegateCommand(() =>
            {
                using (var filedialog = new CommonOpenFileDialog())
                {
                    filedialog.InitialDirectory = Environment.CurrentDirectory;
                    filedialog.Filters.Add(new CommonFileDialogFilter("Xml Files", "*.xml"));
                    filedialog.Filters.Add(new CommonFileDialogFilter("All Files", "*"));
                    if (filedialog.ShowDialog() == CommonFileDialogResult.Ok)
                        Current.MemberwiseCopy(Load(filedialog.FileName));
                }
            })
        };
    }
    public enum LosCalcType { SimpleSum, Formula14Q3, Formula14Q4, Formula16Q1 }
}
