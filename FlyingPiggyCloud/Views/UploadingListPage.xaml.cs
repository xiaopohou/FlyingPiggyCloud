using FlyingPiggyCloud.Controllers.Results.FileSystem;
using FlyingPiggyCloud.Models;
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

namespace FlyingPiggyCloud.Views
{
    /// <summary>
    /// UploadingListPage.xaml 的交互逻辑
    /// </summary>
    public partial class UploadingListPage : Page
    {
        /// <summary>
        /// 下载列表，对该对象的写操作务必在主线程执行
        /// </summary>
        private static ObservableCollection<DownloadTask> DownloadTasks = new ObservableCollection<DownloadTask>();

        private static System.Timers.Timer timer = new System.Timers.Timer(500d);

        public static async Task RefreshStatus()
        {
            timer.Enabled = false;

            List<DownloadTask> Completed = new List<DownloadTask>();
            try
            {
                if (DownloadTasks.Count != 0)
                {
                    DownloadTask[] downloadTasks;
                    lock (DownloadTasks)
                    {
                        downloadTasks = new DownloadTask[DownloadTasks.Count];
                        DownloadTasks.CopyTo(downloadTasks, 0);
                    }

                    foreach (DownloadTask a in downloadTasks)
                    {
                        await a.RefreshStatus();
                        if (a.Status == FlyingAria2c.DownloadTask.TaskAction.Complete)
                        {
                            Completed.Add(a);
                        }
                    }
                    if (Completed.Count != 0)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            lock (DownloadTasks)
                            {
                                foreach (DownloadTask a in Completed)
                                {
                                    DownloadTasks.Remove(a);
                                }

                            }
                        });
                        CompletedListPage.CompletedTasksAddRange(Completed);
                    }
                }
            }
            catch (System.Exception)
            {

            }
            finally
            {
                if (DownloadTasks.Count != 0)
                {
                    timer.Enabled = true;
                }
            }
        }

        public static void NewDownloadTask(FileMetaData fileMetaData)
        {
            DownloadTask downloadTask = new DownloadTask(fileMetaData);
            App.Current.Dispatcher.Invoke(() =>
            {
                lock (DownloadTasks)
                {
                    DownloadTasks.Add(downloadTask);

                }
            });
            timer.Enabled = true;
        }

        public UploadingListPage()
        {
            InitializeComponent();
        }
    }
}
