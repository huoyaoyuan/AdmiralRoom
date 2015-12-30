using System;
using System.Drawing;
using System.Windows.Forms;
using Application = System.Windows.Application;

#pragma warning disable CC0029

namespace Huoyaoyuan.AdmiralRoom
{
    public class BallonNotifier : Notifier, IDisposable
    {
        private NotifyIcon notifyIcon;

        public void Dispose() => notifyIcon.Dispose();

        public override void Initialize()
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
                this.notifyIcon = new NotifyIcon
                {
                    Text = "提督の部屋",
                    Icon = new Icon(stream),
                    Visible = true
                };
            }
        }

        public override void Show(string title, string detail)
        {
            notifyIcon?.ShowBalloonTip(1000, title, detail, ToolTipIcon.None);
        }
    }
}
