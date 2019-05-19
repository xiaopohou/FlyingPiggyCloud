using SixCloudCustomControlLibrary.Controls;
using System.Windows;

namespace SixCloud.Views
{
    /// <summary>
    /// OfflineUrlsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class OfflineUrlsDialog : MetroWindow
    {
        public OfflineUrlsDialog()
        {
            InitializeComponent();
            System.Threading.Tasks.Task.Run(() =>
            {
                System.Threading.Thread.Sleep(1000);
                App.Current.Dispatcher.Invoke(() =>
                {
                    IsInputUrlsStage.IsChecked = true;
                });
            });

        }

        private void CutDownRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
