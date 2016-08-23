using System.Windows;

namespace Huoyaoyuan.AdmiralRoom.Notifier
{
    class MessageBoxNotifierFactory : NotifierFactory
    {
        public override void Show(string title, string detail, string sound = null)
            => MessageBox.Show(Application.Current.MainWindow, detail, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
