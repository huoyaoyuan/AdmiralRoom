using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Huoyaoyuan.AdmiralRoom.Views
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void OpenScreenShotFolder(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(Config.Current.ScreenShotFolder))
                Process.Start(Config.Current.ScreenShotFolder);
        }

        private void SelectScreenFolder(object sender, RoutedEventArgs e)
        {
            using (var folderdialog = new CommonOpenFileDialog())
            {
                folderdialog.IsFolderPicker = true;
                folderdialog.InitialDirectory = Config.Current.ScreenShotFolder;
                if (folderdialog.ShowDialog() == CommonFileDialogResult.Ok)
                    Config.Current.ScreenShotFolder = folderdialog.FileName;
            }
        }
    }
}
