using SixCloud.Controllers;
using SixCloud.Models;
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
            //new LoadingView(this, ()=>Thread.Sleep(5000)).ShowDialog();
            //OfflineDownloader offlineDownloader = new OfflineDownloader();
            //GenericResult<OfflineTaskParseUrl[]> x = offlineDownloader.ParseUrl(new string[]
            //{
            //    @"https://files-cdn.cnblogs.com/files/Gyoung/SqliteLinqTest.zip",
            //    @"https://newcontinuum.dl.sourceforge.net/project/openmediavault/4.1.3/openmediavault_4.1.3-amd64.iso"
            //});
            //var c = offlineDownloader.Add(@"/", new object[]
            //{
            //    new {identity=x.Result[0].Identity},
            //    new {identity=x.Result[1].Identity}
            //});
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //FileDownloader.Stop();
        }
    }
}
