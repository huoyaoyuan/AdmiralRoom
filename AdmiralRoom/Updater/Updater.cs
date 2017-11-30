using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Meowtrix.ComponentModel;
using Newtonsoft.Json.Linq;

#pragma warning disable CC0022

namespace Huoyaoyuan.AdmiralRoom.Updater
{
    sealed class Updater : NotificationObject
    {
        public static string VersionString => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public DelegateCommand CheckCommand { get; }
        public DelegateCommand DownloadCommand { get; }
        public DelegateCommand CancelDownloadCommand { get; }
        public DelegateCommand UpdateFileCommand { get; }
        public DelegateCommand RestartCommand { get; }
        private WebClient downloadwebclient;
        private bool isautoupdate;
        private Updater()
        {
            CheckCommand = new DelegateCommand(async () =>
            {
                CheckCommand.CanExecute = false;
                bool result = await TryFindUpdateAsync();
                Status = result ? UpdaterStatus.Download : UpdaterStatus.Check;
                if (result && isautoupdate)
                {
                    DispatcherHelper.UIDispatcher.Invoke(() =>
                    {
                        if (MessageBox.Show($"{StringTable.Update_Text_Download}{NewVersion}",
                            StringTable.Update, MessageBoxButton.OKCancel, MessageBoxImage.Information)
                            == MessageBoxResult.OK)
                            new AboutWindow { Owner = Application.Current.MainWindow }.ShowDialog();
                    });
                    if (Config.Current.AutoDownloadUpdate) DownloadCommand.Execute(null);
                    else isautoupdate = false;
                }
                else isautoupdate = false;
                CheckCommand.CanExecute = true;
            });
            DownloadCommand = new DelegateCommand(async () =>
            {
                Status = UpdaterStatus.CancelDownload;
                bool result = await DownloadUpdateAsync();
                Status = result ? UpdaterStatus.UpdateFile : UpdaterStatus.Download;
                if (result && isautoupdate) UpdateFileCommand.Execute(null);
                else isautoupdate = false;
            });
            CancelDownloadCommand = new DelegateCommand(() =>
            {
                downloadwebclient?.CancelAsync();
                Status = UpdaterStatus.Download;
            });
            UpdateFileCommand = new DelegateCommand(async () =>
            {
                UpdateFileCommand.CanExecute = false;
                await UpdateFileAsync();
                UpdateFileCommand.CanExecute = true;
                Status = UpdaterStatus.Restart;
                isautoupdate = false;
            });
            RestartCommand = new DelegateCommand(Restart);
            Timer.Elapsed += (_, __) =>
            {
                if (Status == UpdaterStatus.Check && Config.Current.AutoCheckUpdate)
                {
                    isautoupdate = true;
                    DispatcherHelper.UIDispatcher.Invoke(() => CheckCommand.Execute(null));
                }
            };
            isautoupdate = true;
            CheckCommand.Execute(null);
        }
        public Timer Timer { get; } = new Timer(3600 * 12 * 1000);
        public static Updater Instance { get; } = new Updater();
        private Uri updateurl;
        private string downloadfilename;

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

        #region Status
        private UpdaterStatus _status = UpdaterStatus.Check;
        public UpdaterStatus Status
        {
            get { return _status; }
            private set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public async Task<bool> TryFindUpdateAsync()
        {
            try
            {
                JObject obj;
                var wrq = WebRequest.CreateHttp("https://api.github.com/repos/huoyaoyuan/AdmiralRoom/releases/latest");
                wrq.Method = "GET";
                wrq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Safari/537.36 Edge/13.10586";
                var proxy = Config.Current.GetHTTPSProxy();
                if (Config.Current.UpdateUsingProxy && proxy != null)
                    wrq.Proxy = new WebProxy($"http://{proxy.Host}:{proxy.Port}");
                using (var wrs = await wrq.GetResponseAsync())
                using (var reader = new StreamReader(wrs.GetResponseStream()))
                    obj = JObject.Parse(reader.ReadToEnd());
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

        public async Task<bool> DownloadUpdateAsync()
        {
            try
            {
                downloadwebclient = new WebClient();
                var proxy = Config.Current.GetHTTPSProxy();
                if (Config.Current.UpdateUsingProxy && proxy != null)
                    downloadwebclient.Proxy = new WebProxy($"http://{proxy.Host}:{proxy.Port}");
                DownloadPercentage = 0;
                downloadwebclient.DownloadProgressChanged += (s, e) => DownloadPercentage = e.ProgressPercentage;
                await downloadwebclient.DownloadFileTaskAsync(updateurl, downloadfilename);
                return true;
            }
            catch { return false; }
            finally
            {
                downloadwebclient?.Dispose();
                downloadwebclient = null;
            }
        }

        public Task UpdateFileAsync() => Task.Factory.StartNew(() =>
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var rootfolder = new DirectoryInfo(".");
            foreach (var file in rootfolder.GetFiles("*.exe", SearchOption.AllDirectories)
                                           .Concat(rootfolder.GetFiles("*.dll", SearchOption.AllDirectories))
                                           .Concat(rootfolder.GetFiles("*.config", SearchOption.AllDirectories)))
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
    enum UpdaterStatus { Check, Download, CancelDownload, UpdateFile, Restart }
}
