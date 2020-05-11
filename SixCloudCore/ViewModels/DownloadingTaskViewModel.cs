using QingzhenyunApis.Methods.V3;
using QingzhenyunApis.Utils;
using SixCloudCore.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace SixCloudCore.ViewModels
{
    internal class DownloadingTaskViewModel : ViewModelBase, ITransferItemViewModel
    {
        private readonly DownloadTask downloadTask;
        private long lastCompletedSize = 0;
        private int retryTimes = 0;

        public string Icon { get; } = "\uf019";

        public string Name => downloadTask.Name;

        public string CurrentFileFullPath => downloadTask.CurrentFileFullPath;

        public string Completed => downloadTask.Completed;

        public string TargetUUID { get; private set; }

        public string SavedLocalPath { get; private set; }

        public string Total => downloadTask.Total;

        public double Progress => downloadTask.DownloadProgress;

        public TransferTaskStatus Status => downloadTask.Status;

        public string Speed => downloadTask.Speed;

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
        private void Pause(object parameter)
        {
            downloadTask.Pause();
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
            downloadTask.Stop();
            DownloadCanceled?.Invoke(this, EventArgs.Empty);
        }



        public event EventHandler<EventArgs> DownloadCompleted;

        public event EventHandler<EventArgs> DownloadCanceled;


        public DownloadingTaskViewModel(string targetUUID, string localPath, string name)
        {
            TargetUUID = targetUUID;
            SavedLocalPath = localPath;
            RecoveryCommand = new DependencyCommand(Recovery, CanRecovery);
            PauseCommand = new DependencyCommand(Pause, CanPause);
            CancelCommand = new DependencyCommand(Cancel, DependencyCommand.AlwaysCan);

            downloadTask = new DownloadTask(localPath, name, TargetUUID, (sender, e) =>
             {
                 DownloadCompleted?.Invoke(this, e);
             });

            WeakEventManager<DispatcherTimer, EventArgs>.AddHandler(ITransferItemViewModel.timer, nameof(ITransferItemViewModel.timer.Tick), Callback);

            void Callback(object sender, EventArgs e)
            {
                if (lastCompletedSize == downloadTask.CompletedBytes)
                {
                    retryTimes++;
                }
                else
                {
                    lastCompletedSize = downloadTask.CompletedBytes;
                }

                if (retryTimes >= 60)
                {
                    retryTimes = 0;
                    downloadTask.Redownload();
                }

                OnPropertyChanged(nameof(Completed));
                OnPropertyChanged(nameof(Speed));
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(Progress));
            }
        }

    }

    public class StatusToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((TransferTaskStatus)value) == TransferTaskStatus.Running ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
