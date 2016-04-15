using System;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Huoyaoyuan.AdmiralRoom.Updater
{
    class Updater : NotificationObject
    {
        private Updater() { }
        public Updater Instance { get; } = new Updater();
        private Uri updateurl;

        #region NewVersion
        private Version _newversion;
        public Version NewVersion
        {
            get { return _newversion; }
            private set
            {
                if (_newversion != value)
                {
                    _newversion = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region ReleaseTime
        private DateTimeOffset _releasetime;
        public DateTimeOffset ReleaseTime
        {
            get { return _releasetime; }
            private set
            {
                if (_releasetime != value)
                {
                    _releasetime = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public async Task<bool> TryFindUpdateAsync()
        {
            try
            {
                string api;
                using (var webclient = new WebClient())
                    api = await webclient.DownloadStringTaskAsync("https://api.github.com/repos/huoyaoyuan/AdmiralRoom/releases/latest");
                var obj = JObject.Parse(api);
                NewVersion = new Version(Regex.Match((string)obj["name"], @"\d+(\.\d+)+").Value);
                if (NewVersion <= Assembly.GetExecutingAssembly().GetName().Version)
                {
                    updateurl = null;
                    return false;
                }
                updateurl = new Uri((string)obj["assets"][0]["browser_download_url"]);
                ReleaseTime = (DateTimeOffset)obj["published_at"];
                return true;
            }
            catch
            {
                updateurl = null;
                return false;
            }
        }
    }
}
