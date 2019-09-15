﻿using SixCloud.Controllers;
using SixCloud.Models;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace SixCloud.ViewModels
{
    internal class DownloadingTaskViewModel : ViewModelBase
    {
        //private readonly DownloadingListViewModel ParentContainer;

        private readonly DownloadTask downloadTask;

        public string Icon { get; } = "\uf019";

        public string Name => downloadTask.Name;

        public string CurrentFileFullPath => downloadTask.CurrentFileFullPath;

        public string Completed => downloadTask.Completed;

        public string DownloadAddress => downloadTask.DownloadAddress;

        public string TargetUUID { get; private set; }

        public string SavedLocalPath { get; private set; }

        public string Total => downloadTask.Total;

        public double Progress => downloadTask.DownloadProgress;

        public DownloadTask.TaskStatus Status => downloadTask.Status;

        public async void Start()
        {
            await downloadTask.Start();
            OnPropertyChanged(nameof(Status));
        }

        public void Stop()
        {
            DownloadCompleted?.Invoke(this, new EventArgs());
            downloadTask.Stop();
            OnPropertyChanged(nameof(Status));
        }

        public async void Pause()
        {
            await downloadTask.Pause();
            OnPropertyChanged(nameof(Status));
        }

        public event EventHandler<EventArgs> DownloadCompleted;

        public DownloadingTaskViewModel(string targetUUID, string downloadAddress, string localPath, string name)
        {
            TargetUUID = targetUUID;
            SavedLocalPath = localPath;
            downloadTask = new DownloadTask(downloadAddress, localPath, name);
            //TasksLogger.AddRecord(downloadTask);
            downloadTask.DownloadFileProgressChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(Completed));
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(Progress));
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
