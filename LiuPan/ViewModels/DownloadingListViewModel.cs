using SixCloud.Models;
using System.Collections.ObjectModel;

namespace SixCloud.ViewModels
{
    internal class DownloadingListViewModel : ViewModelBase
    {
        private static readonly ObservableCollection<DownloadingTask> _observableCollection = new ObservableCollection<DownloadingTask>();

        public static void NewTask(string downloadAddress, string localPath, bool isAutoStart = true)
        {
            DownloadingTask task = new DownloadingTask(downloadAddress, localPath);
            if (isAutoStart)
            {
                task.Start();
            }
            App.Current.Dispatcher.Invoke(() =>
            {
                _observableCollection.Add(task);
            });
        }

        public ObservableCollection<DownloadingTask> ObservableCollection => _observableCollection;

        internal class DownloadingTask : ViewModelBase
        {
            private readonly DownloadTask downloadTask;

            public string Name => downloadTask.Name;

            public string Completed => downloadTask.Completed;

            public string Total => downloadTask.Total;

            public double Progress => downloadTask.DownloadProgress;

            public int Status => downloadTask.S;

            public void Start()
            {
                downloadTask.Start();
            }

            public void Stop()
            {
                downloadTask.Stop();
            }

            public void Pause()
            {
                downloadTask.Pause();
            }

            public DownloadingTask(string downloadAddress, string localPath)
            {
                downloadTask = new DownloadTask(downloadAddress, localPath);
                downloadTask.DownloadFileProgressChanged += (sender, e) =>
                {
                    OnPropertyChanged("Completed");
                    OnPropertyChanged("Total");
                    OnPropertyChanged("Progress");
                };
            }

        }
    }


}
