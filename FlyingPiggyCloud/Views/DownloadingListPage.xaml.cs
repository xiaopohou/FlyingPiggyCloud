using FlyingPiggyCloud.Controllers.Results.FileSystem;
using FlyingPiggyCloud.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace FlyingPiggyCloud.Views
{
    /// <summary>
    /// DownloadingListPage.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadingListPage : Page
    {
        /// <summary>
        /// 下载列表，对该对象的写操作务必在主线程执行
        /// </summary>
        private static ObservableCollection<DownloadTask> DownloadTasks = new ObservableCollection<Models.DownloadTask>();

        private static System.Timers.Timer refreshTimer = new System.Timers.Timer(500d);

        public static async Task RefreshStatus()
        {
            refreshTimer.Enabled = false;

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
                refreshTimer.Enabled = true;
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
        }

        static DownloadingListPage()
        {
            refreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(async (sender, e) => await RefreshStatus());
            refreshTimer.Enabled = true;
        }

        public DownloadingListPage()
        {
            InitializeComponent();
            DownloadList.ItemsSource = DownloadTasks;
        }

        private async void Stop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var task = (DownloadTask)btn.DataContext;
            await task.Stop();
        }

        private async void Pause_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var task = (DownloadTask)btn.DataContext;
            if (task.Status==FlyingAria2c.DownloadTask.TaskAction.Paused)
            {
                await task.Start();
            }
            else
            {
                await task.Pause();
            }
        }

        private async void PauseAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var list = DownloadList.SelectedItems;
            if(list.Count!=0)
            {
                foreach (DownloadTask downloadTask in list)
                {
                    await downloadTask.Pause();
                }
            }
            else
            {
                await FlyingAria2c.DownloadTask.PauseAll();
            }
            DownloadList.SelectedItem = null;
            
        }

        private async void StartAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            await FlyingAria2c.DownloadTask.StartAll();
            DownloadList.SelectedItem = null;
        }
    }
}