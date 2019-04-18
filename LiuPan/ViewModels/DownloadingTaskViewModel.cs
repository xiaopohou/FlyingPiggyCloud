using SixCloud.Controllers;
using SixCloud.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

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

        public async void Start()
        {
            await downloadTask.Start();
            OnPropertyChanged("Status");
        }

        public void Stop()
        {
            DownloadCompleted?.Invoke(this, new EventArgs());
            downloadTask.Stop();
            OnPropertyChanged("Status");
        }

        public async void Pause()
        {
            await downloadTask.Pause();
            OnPropertyChanged("Status");
        }

        public event EventHandler<EventArgs> DownloadCompleted;

        public DownloadingTaskViewModel(string downloadAddress, string localPath, string name)
        {
            downloadTask = new DownloadTask(downloadAddress, localPath, name);
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
                    DownloadCompleted?.Invoke(this, new EventArgs());
                }
            };
        }

    }

    public class StatusToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DownloadTask.TaskStatus)value) == DownloadTask.TaskStatus.Running ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
