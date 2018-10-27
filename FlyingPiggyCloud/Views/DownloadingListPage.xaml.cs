using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FlyingPiggyCloud.Views
{
    /// <summary>
    /// DownloadingListPage.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadingListPage : Page
    {
        private static ObservableCollection<ViewModels.DownloadTask> DownloadTasks = new ObservableCollection<ViewModels.DownloadTask>();

        public DownloadingListPage()
        {
            InitializeComponent();
            DownloadList.ItemsSource = DownloadTasks;
            DownloadTasks.Add(new ViewModels.DownloadTask(new Controllers.Results.FileSystem.FileMetaData
            {
                Name="测试下载",
                DownloadAddress= "https://dl.twrp.me/oneplus3/twrp-3.2.3-0-oneplus3.img"
            }));
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += (sender, e) =>
            {
                lock(DownloadTasks)
                {
                    foreach (ViewModels.DownloadTask a in DownloadTasks)
                    {
                        a.RefreshStatus();
                    }
                }
                
            };
            dispatcherTimer.Start();
        }
    }
}
