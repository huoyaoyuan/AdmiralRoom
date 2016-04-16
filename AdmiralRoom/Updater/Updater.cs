using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace Huoyaoyuan.AdmiralRoom.Updater
{
    class Updater : NotificationObject
    {
        private Updater() { }
        public static Updater Instance { get; } = new Updater();
        public static readonly string[] ProtectedFolders = { "logs", "information", "modules" };
        private Uri updateurl;
        private string downloadfilename;
        private bool downloadcompleted;

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

        #region DownloadPercentage
        private int _downloadpercentage;
        public int DownloadPercentage
        {
            get { return _downloadpercentage; }
            private set
            {
                if (_downloadpercentage != value)
                {
                    _downloadpercentage = value;
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
                downloadfilename = (string)obj["assets"][0]["name"];
                ReleaseTime = (DateTimeOffset)obj["published_at"];
                return true;
            }
            catch
            {
                updateurl = null;
                return false;
            }
        }

        public async Task DownloadUpdateAsync()
        {
            using (var webclient = new WebClient())
            {
                webclient.DownloadProgressChanged += (s, e) => DownloadPercentage = e.ProgressPercentage;
                await webclient.DownloadFileTaskAsync(updateurl, downloadfilename);
                downloadcompleted = true;
            }
        }

        public Task UpdateFileAsync() => Task.Factory.StartNew(() =>
            {
                Environment.CurrentDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                var rootfolder = new DirectoryInfo(".");
                foreach (var file in rootfolder.GetFiles())
                    if (file.Extension != "xml" && file.Name != downloadfilename)
                        file.MoveTo(file.FullName + ".old");
                foreach (var folder in rootfolder.GetDirectories())
                    if (!ProtectedFolders.Contains(folder.Name.ToLowerInvariant()))
                        foreach (var file in folder.GetFiles())
                            file.MoveTo(file.FullName + ".old");
                using (var zip = ZipFile.OpenRead(downloadfilename))
                    foreach (var entry in zip.Entries)
                    {
                        string name = entry.FullName;
                        name = name.Substring(name.IndexOf(Path.DirectorySeparatorChar) + 1);
                        using (var entrystream = entry.Open())
                        {
                            FileStream filestream = null;
                            try
                            {
                                filestream = File.OpenWrite(name);
                            }
                            catch
                            {
                                File.Move(name, name + ".old");
                                filestream = File.OpenWrite(name);
                            }
                            entrystream.CopyTo(filestream);
                            filestream.Flush();
                            filestream.Dispose();
                        }
                    }
                File.Delete(downloadfilename);
            });

        public void Restart()
        {
            Application.Current.Exit += (_, __) => Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Application.Current.Shutdown();
        }
    }
}
