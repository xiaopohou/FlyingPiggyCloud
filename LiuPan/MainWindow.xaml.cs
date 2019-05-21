using SixCloud.Controllers;
using SixCloud.Models;
using SixCloud.ViewModels;
using SixCloud.Views;
using SixCloudCustomControlLibrary.Controls;
using System.Windows;

namespace SixCloud
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
            new LoginView().Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //FileDownloader.Start();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var a = new OfflineUrlsDialog();
            a.Show();
            a.DataContext = new OfflineUrlsDialogViewModel();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //FileDownloader.Stop();
        }
    }
}
