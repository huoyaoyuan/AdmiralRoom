using System;
using System.Diagnostics;
using System.IO;
using Huoyaoyuan.AdmiralRoom.ShellHelpers;
using MS.WindowsAPICodePack.Internal;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace Huoyaoyuan.AdmiralRoom
{
    public class ToastNotifier : Notifier
    {
        private static readonly string APP_ID = "Huoyaoyuan.AdmiralRoom";
        public static bool IsSupported
        {
            get
            {
                var version = Environment.OSVersion.Version;
                return (version.Major == 6 && version.Minor >= 2) || version.Major > 6;
            }
        }
        public override void Initialize() => TryCreateShortcut();
        private static bool TryCreateShortcut()
        {
            string shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Windows\\Start Menu\\Programs\\提督の部屋.lnk";
            if (!File.Exists(shortcutPath))
            {
                InstallShortcut(shortcutPath);
                return true;
            }
            return false;
        }
        public static void InstallShortcut(string shortcutPath)
        {
            // Find the path to the current executable
            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            IShellLinkW newShortcut = (IShellLinkW)new CShellLink();

            // Create a shortcut to the exe
            ErrorHelper.VerifySucceeded(newShortcut.SetPath(exePath));
            ErrorHelper.VerifySucceeded(newShortcut.SetArguments(""));

            // Open the shortcut property store, set the AppUserModelId property
            IPropertyStore newShortcutProperties = (IPropertyStore)newShortcut;

            using (PropVariant appId = new PropVariant(APP_ID))
            {
                ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(SystemProperties.System.AppUserModel.ID, appId));
                ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
            }

            // Commit the shortcut to disk
            IPersistFile newShortcutSave = (IPersistFile)newShortcut;

            ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
        }
        public override void Show(string title, string detail)
        {
            // Get a toast XML template
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            // Fill in the text elements
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            //for (int i = 0; i < stringElements.Length; i++)
            //{
            //    stringElements[i].AppendChild(toastXml.CreateTextNode("Line " + i));
            //}
            stringElements[0].AppendChild(toastXml.CreateTextNode(title));
            stringElements[1].AppendChild(toastXml.CreateTextNode(detail));

            // Create the toast and attach event listeners
            ToastNotification toast = new ToastNotification(toastXml);
            //toast.Activated += ToastActivated;
            //toast.Dismissed += ToastDismissed;
            //toast.Failed += ToastFailed;

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
        }
        //private void ToastActivated(ToastNotification sender, object e)
        //{
            
        //}

        //private void ToastDismissed(ToastNotification sender, ToastDismissedEventArgs e)
        //{
        //    string outputText = "";
        //    switch (e.Reason)
        //    {
        //        case ToastDismissalReason.ApplicationHidden:
        //            outputText = "The app hid the toast using ToastNotifier.Hide";
        //            break;
        //        case ToastDismissalReason.UserCanceled:
        //            outputText = "The user dismissed the toast";
        //            break;
        //        case ToastDismissalReason.TimedOut:
        //            outputText = "The toast has timed out";
        //            break;
        //    }
        //    System.Windows.MessageBox.Show(outputText);
        //}

        //private void ToastFailed(ToastNotification sender, ToastFailedEventArgs e)
        //{
            
        //}
    }
}
