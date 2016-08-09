using Meowtrix.ComponentModel;

namespace Huoyaoyuan.AdmiralRoom.Models
{
    sealed class Status : NotificationObject
    {
        private Status() { }
        public static Status Current { get; } = new Status();

        private Volume _volume;
        public Volume Volume
        {
            get
            {
                if (_volume == null) _volume = Volume.GetInstance();
                return _volume;
            }
        }

        #region StatusText
        private string _statustext = "";
        public string StatusText
        {
            get { return _statustext; }
            set
            {
                if (_statustext != value)
                {
                    _statustext = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region IsGameLoaded
        private bool _isgameloaded;
        public bool IsGameLoaded
        {
            get { return _isgameloaded; }
            set
            {
                if (_isgameloaded != value)
                {
                    _isgameloaded = value;
                    OnAllPropertyChanged();
                }
            }
        }
        #endregion
    }
}
