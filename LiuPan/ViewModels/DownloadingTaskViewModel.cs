using SixCloud.Controllers;
using SixCloud.Models;
using System;
using System.Windows.Input;

namespace SixCloud.ViewModels
{
    internal class DownloadingTaskViewModel : ViewModelBase
    {
        //private readonly DownloadingListViewModel ParentContainer;

        private readonly DownloadTask downloadTask;

        public string Name => downloadTask.Name;

        public string Completed => downloadTask.Completed;

        public string Total => downloadTask.Total;

        public double Progress => downloadTask.DownloadProgress;

        public DownloadTask.TaskStatus Status => downloadTask.Status;

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

        public event EventHandler<EventArgs> DownloadCompleted;

        public DownloadingTaskViewModel(string downloadAddress, string localPath)
        {
            downloadTask = new DownloadTask(downloadAddress, localPath);
            TransmissionProgressController.DownloadingCache.AddRecord(downloadTask);
            downloadTask.DownloadFileProgressChanged += (sender, e) =>
            {
                OnPropertyChanged("Completed");
                OnPropertyChanged("Total");
                OnPropertyChanged("Progress");
            };
            downloadTask.DownloadFileCompleted += (sender, e) =>
            {
                if (e.State == FileDownloader.CompletedState.Succeeded)
                {
                    DownloadCompleted(this, new EventArgs());
                }
            };
        }

    }
}
