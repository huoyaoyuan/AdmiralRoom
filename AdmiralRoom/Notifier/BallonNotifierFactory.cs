using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media;
using Application = System.Windows.Application;

#pragma warning disable CC0029

namespace Huoyaoyuan.AdmiralRoom.Notifier
{
    public class BallonNotifierFactory : NotifierFactory, IDisposable
    {
        private NotifyIcon notifyIcon;

        public void Dispose() => notifyIcon.Dispose();

        private readonly MediaPlayer player = new MediaPlayer();

        public static bool IsSupported => true;

        public BallonNotifierFactory()
        {
            const string iconUri = "pack://application:,,,/AdmiralRoom;Component/Icons/app.ico";

            Uri uri;
            if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri))
                return;

            var streamResourceInfo = Application.GetResourceStream(uri);
            if (streamResourceInfo == null)
                return;

            using (var stream = streamResourceInfo.Stream)
            {
                notifyIcon = new NotifyIcon
                {
                    Text = "提督の部屋",
                    Icon = new Icon(stream),
                    Visible = true
                };
            }
        }

        public override void Show(string title, string detail, string sound = null)
        {
            notifyIcon?.ShowBalloonTip(1000, title, detail, ToolTipIcon.None);
            if (!string.IsNullOrEmpty(sound))
            {
                DispatcherHelper.UIDispatcher.Invoke(() =>
                {
                    try
                    {
                        player.Open(new Uri(sound, UriKind.Relative));
                        player.Play();
                    }
                    catch { }
                });
            }
        }
    }
}
