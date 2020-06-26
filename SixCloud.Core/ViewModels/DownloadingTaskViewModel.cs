using QingzhenyunApis.Methods.V3;
using SixCloud.Core.Models;
using SixCloudCore.SixTransporter.Downloader;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SixCloud.Core.ViewModels
{
    public abstract class DownloadingTaskViewModel : ViewModelBase, ITransferItemViewModel
    {
        /// <summary>
        /// 创建一个下载器
        /// </summary>
        /// <param name="downloadPath"></param>
        /// <param name="downloadUrl"></param>
        /// <returns></returns>
        protected HttpDownloader CreateHttpDownloader(string downloadPath, string downloadUrl, string targetUUID)
        {
            DownloadTaskInfo taskInfo = File.Exists(downloadPath + ".downloading") ? DownloadTaskInfo.Load(downloadPath + ".downloading") : new DownloadTaskInfo()
            {
                DownloadUrl = downloadUrl, // 下载链接，可以为null，任务开始前再赋值初始化
                DownloadPath = downloadPath,
                Threads = 4,
            };

            HttpDownloader httpDownloader = new HttpDownloader(taskInfo); // 下载默认会在StartDownload函数初始化, 保存下载进度文件到file.downloading文件

            httpDownloader.DownloadStatusChangedEvent += async (oldValue, newValue, sender) =>
            {
                if (newValue == DownloadStatusEnum.Failed)
                {
                    await DownloadingFailedHandler(taskInfo, httpDownloader, targetUUID);
                }
                OnPropertyChanged(nameof(Status));
            };
            return httpDownloader;
        }

        public string Icon { get; } = "\uf019";

        public abstract string Name { get; protected set; }

        public abstract string CurrentFileFullPath { get; }

        public abstract string Completed { get; }

        public abstract string TargetUUID { get; protected set; }

        public abstract string SavedLocalPath { get; protected set; }

        public abstract string Total { get; }

        public abstract double Progress { get; }

        public abstract TransferTaskStatus Status { get; }

        public abstract string Speed { get; }

        public DependencyCommand RecoveryCommand { get; }
        protected abstract void Recovery(object parameter);

        private bool CanRecovery(object parameter)
        {
            return Status == TransferTaskStatus.Pause;
        }


        public DependencyCommand PauseCommand { get; }
        protected abstract void Pause(object parameter);
        private bool CanPause(object parameter)
        {
            return Status == TransferTaskStatus.Running;
        }


        public DependencyCommand CancelCommand { get; }
        protected bool Cancelled { get; set; } = false;
        protected abstract void Cancel(object parameter);

        private async Task DownloadingFailedHandler(DownloadTaskInfo taskInfo, HttpDownloader httpDownloader, string targetUUID)
        {
            Thread.Sleep(TimeSpan.FromMinutes(1));
            if (!Cancelled)
            {
                taskInfo.DownloadUrl = (await FileSystem.GetDownloadUrlByIdentity(targetUUID)).DownloadAddress; ;
                await Task.Run(() => httpDownloader?.StartDownload());
            }
        }

        public virtual event EventHandler DownloadCompleted;

        public virtual event EventHandler DownloadCanceled;

        protected DownloadingTaskViewModel()
        {
            RecoveryCommand = new DependencyCommand(Recovery, CanRecovery);
            PauseCommand = new DependencyCommand(Pause, CanPause);
            CancelCommand = new DependencyCommand(Cancel, DependencyCommand.AlwaysCan);
        }

    }

    public class DownloadingTaskInGroupViewModel : ViewModelBase
    {
        public DownloadTaskGroup Owner { get; }

        public DownloadTaskRecord Record { get; }

        public string Name => Record.Name;

        public string LocalPath => Record.LocalPath;

        public double Progress => throw new NotImplementedException();

        public DownloadTaskStatusInGroup StatusInGroup => throw new NotImplementedException();

        internal DownloadingTaskInGroupViewModel(DownloadTaskGroup downloadTaskGroup, DownloadTaskRecord downloadTaskRecord)
        {
            Owner = downloadTaskGroup;
            Record = downloadTaskRecord;
        }
    }

    public enum DownloadTaskStatusInGroup
    {
        Running,
        Waitting,
        Completed
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
