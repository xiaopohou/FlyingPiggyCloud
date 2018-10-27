using FlyingPiggyCloud.Controllers.Results.FileSystem;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FlyingPiggyCloud.Views
{
    /// <summary>
    /// DownloadingListPage.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadingListPage : Page
    {
        private static ObservableCollection<ViewModels.DownloadTask> DownloadTasks = new ObservableCollection<ViewModels.DownloadTask>();

        private static System.Timers.Timer timer = new System.Timers.Timer(500d);

        public static async Task RefreshStatus()
        {
            timer.Enabled = false;

            List<ViewModels.DownloadTask> Completed = new List<ViewModels.DownloadTask>();
            if (DownloadTasks.Count != 0)
            {
                foreach (ViewModels.DownloadTask a in DownloadTasks)
                {
                    await a.RefreshStatus();
                    if (a.Status == FlyingAria2c.DownloadTask.TaskAction.Complete)
                    {
                        Completed.Add(a);
                    }
                }
                if (Completed.Count == 0)
                {
                    timer.Enabled = true;
                }
                else
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        lock (DownloadTasks)
                        {
                            foreach (ViewModels.DownloadTask a in Completed)
                            {
                                DownloadTasks.Remove(a);
                            }
                            if (DownloadTasks.Count != 0)
                            {
                                timer.Enabled = true;
                            }
                        }
                        CompletedListPage.CompletedTasksAddRange(Completed);
                    });
                }
            }
        }

        public static void NewDownloadTask(FileMetaData fileMetaData)
        {
            ViewModels.DownloadTask downloadTask = new ViewModels.DownloadTask(fileMetaData);
            lock (DownloadTasks)
            {
                DownloadTasks.Add(downloadTask);
            }
            timer.Enabled = true;
        }

        static DownloadingListPage()
        {
            timer.Elapsed += new System.Timers.ElapsedEventHandler(async (sender, e) =>
              {
                  await RefreshStatus();
              });
        }

        public DownloadingListPage()
        {
            InitializeComponent();
            DownloadList.ItemsSource = DownloadTasks;
        }
    }
}