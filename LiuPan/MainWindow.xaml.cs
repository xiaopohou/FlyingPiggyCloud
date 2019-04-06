using FileDownloader;
using SixCloud.Models;
using SixCloudCustomControlLibrary.Controls;
using System.Windows;
using System.Windows.Media;

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

        private DownloadTask FileDownloader = new DownloadTask("https://download.virtualbox.org/virtualbox/6.0.4/VirtualBox-6.0.4-128413-Win.exe", @"C:\Users\Bsens\Downloads");

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new Views.LoginView().Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FileDownloader.Start();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FileDownloader.Pause();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FileDownloader.Stop();
        }
    }
}
