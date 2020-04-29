using QingzhenyunApis.Methods;
using QingzhenyunApis.Methods.V3;
using SixCloudCore.Controllers;
using SixCloudCore.Models;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace SixCloudCore.ViewModels
{
    internal class DownloadingTaskViewModel : ViewModelBase, ITransferItemViewModel
    {
        private readonly DownloadTask downloadTask;

        public string Icon { get; } = "\uf019";

        public string Name => downloadTask.Name;

        public string CurrentFileFullPath => downloadTask.CurrentFileFullPath;

        public string Completed => downloadTask.Completed;

        public string TargetUUID { get; private set; }

        public string SavedLocalPath { get; private set; }

        public string Total => downloadTask.Total;

        public double Progress => downloadTask.DownloadProgress;

        public TransferTaskStatus Status => downloadTask.Status switch
        {
            DownloadTask.TaskStatus.Running => TransferTaskStatus.Running,
            DownloadTask.TaskStatus.Pause => TransferTaskStatus.Pause,
            DownloadTask.TaskStatus.Cancel => TransferTaskStatus.Stop,
            _ => TransferTaskStatus.Stop
        };

        public string Speed => throw new NotImplementedException();

        public DependencyCommand RecoveryCommand { get; }
        private void Recovery(object parameter)
        {
            downloadTask.Start();
            OnPropertyChanged(nameof(Status));
            RecoveryCommand.OnCanExecutedChanged(this, null);
            PauseCommand.OnCanExecutedChanged(this, null);
        }
        private bool CanRecovery(object parameter)
        {
            return Status == TransferTaskStatus.Pause;
        }


        public DependencyCommand PauseCommand { get; }
        private async void Pause(object parameter)
        {
            await downloadTask.Pause();
            OnPropertyChanged(nameof(Status));
            RecoveryCommand.OnCanExecutedChanged(this, null);
            PauseCommand.OnCanExecutedChanged(this, null);
        }
        private bool CanPause(object parameter)
        {
            return Status == TransferTaskStatus.Running;
        }


        public DependencyCommand CancelCommand { get; }
        private void Cancel(object parameter)
        {
            DownloadCompleted?.Invoke(this, new EventArgs());
            downloadTask.Stop();
            OnPropertyChanged(nameof(Status));
            RecoveryCommand.OnCanExecutedChanged(this, null);
            PauseCommand.OnCanExecutedChanged(this, null);
        }



        public event EventHandler<EventArgs> DownloadCompleted;

        public DownloadingTaskViewModel(string targetUUID, string localPath, string name)
        {
            TargetUUID = targetUUID;
            SavedLocalPath = localPath;
            RecoveryCommand = new DependencyCommand(Recovery, CanRecovery);
            PauseCommand = new DependencyCommand(Pause, CanPause);
            CancelCommand = new DependencyCommand(Cancel, DependencyCommand.AlwaysCan);

            downloadTask = new DownloadTask(localPath, name, () =>
             {
                 return new Uri(FileSystem.GetDownloadUrlByIdentity(targetUUID).Result.DownloadAddress);
             }, (sender, e) =>
             {
                 if (e.State == FileDownloader.CompletedState.Succeeded)
                 {
                     DownloadCompleted?.Invoke(this, new EventArgs());
                 }
             }, (sender, e) =>
              {
                  OnPropertyChanged(nameof(Completed));
                  OnPropertyChanged(nameof(Total));
                  OnPropertyChanged(nameof(Progress));
              });
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
