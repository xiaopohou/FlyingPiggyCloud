using Newtonsoft.Json;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using QingzhenyunApis.Utils;
using SixCloudCore.SixTransporter.Downloader;
using SixCloudCore.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SixCloudCore.Models
{
    internal class DownloadTask : DownloadingTaskViewModel
    {
        private long lastCompletedSize = 0;
        private int retryTimes = 0;
        private HttpDownloader fileDownloader;

        protected string Url { get; private set; }

        protected string Path { get; }

        public override string Name { get; protected set; }

        public override string CurrentFileFullPath => fileDownloader?.Info.DownloadPath;

        public override string Completed => Calculators.SizeCalculator(fileDownloader?.Info.DownloadedSize ?? 0);

        public override string TargetUUID { get; protected set; }

        public override string SavedLocalPath { get; protected set; }

        public override string Total => Calculators.SizeCalculator(fileDownloader?.Info.ContentSize ?? 0);

        public override double Progress => fileDownloader?.DownloadPercentage ?? 0;

        public override TransferTaskStatus Status => (fileDownloader?.Status ?? DownloadStatusEnum.Waiting) switch
        {
            DownloadStatusEnum.Downloading => TransferTaskStatus.Running,
            DownloadStatusEnum.Waiting => TransferTaskStatus.Running,

            DownloadStatusEnum.Paused => TransferTaskStatus.Pause,
            DownloadStatusEnum.Failed => TransferTaskStatus.Pause,
            DownloadStatusEnum.Completed => TransferTaskStatus.Completed,
            _ => throw new InvalidCastException()
        };

        public override string Speed => Calculators.SizeCalculator(fileDownloader?.Speed ?? 0) + "/秒";

        public long CompletedBytes => fileDownloader?.Info.DownloadedSize ?? 0;

        protected override async void Recovery(object parameter = null)
        {
            if (fileDownloader?.Status == DownloadStatusEnum.Downloading)
            {
                return;
            }

            if (Url == null)
            {
                Url = (await FileSystem.GetDownloadUrlByIdentity(TargetUUID)).DownloadAddress;
            }

            if (fileDownloader == null || fileDownloader.Status == DownloadStatusEnum.Failed)
            {
                string downloadPath = System.IO.Path.Combine(Path, Name);
                DownloadTaskInfo taskInfo;

                if (File.Exists(downloadPath + ".downloading"))
                {
                    taskInfo = DownloadTaskInfo.Load(downloadPath + ".downloading");
                }
                else
                {
                    taskInfo = new DownloadTaskInfo()
                    {
                        DownloadUrl = Url, // 下载链接，可以为null，任务开始前再赋值初始化
                        DownloadPath = downloadPath,
                        Threads = 4,
                    };
                }
                fileDownloader = new HttpDownloader(taskInfo); // 下载默认会在StartDownload函数初始化, 保存下载进度文件到file.downloading文件
                fileDownloader.DownloadStatusChangedEvent += (oldValue, newValue, sender) =>
                {
                    if (newValue == DownloadStatusEnum.Completed)
                    {
                        DownloadCompleted?.Invoke(sender, null);
                    }
                };
            }

            await Task.Run(() => fileDownloader?.StartDownload());

            OnPropertyChanged(nameof(Status));
            RecoveryCommand.OnCanExecutedChanged(this, null);
            PauseCommand.OnCanExecutedChanged(this, null);
        }

        protected override void Pause(object parameter = null)
        {
            if (Status != TransferTaskStatus.Running)
            {
                return;
            }

            try
            {
                fileDownloader.StopAndSave()?.Save(System.IO.Path.Combine(Path, $"{Name}.downloading"));
            }
            catch (NullReferenceException ex)
            {
                ex.ToSentry().AttachExtraInfo(nameof(DownloadTask), this).Submit();
            }

            OnPropertyChanged(nameof(Status));
            RecoveryCommand.OnCanExecutedChanged(this, null);
            PauseCommand.OnCanExecutedChanged(this, null);
        }

        protected override void Cancel(object parameter = null)
        {
            fileDownloader.StopAndSave(true)?.Save(System.IO.Path.Combine(Path, $"{Name}.downloading"));
            try
            {
                File.Delete(System.IO.Path.Combine(Path, Name));
            }
            catch (IOException ex)
            {
                ex.Submit();
            }
            try
            {
                File.Delete(System.IO.Path.Combine(Path, $"{Name}.downloading"));
            }
            catch (IOException ex)
            {
                ex.Submit();
            }

            DownloadCanceled?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 删除本地文件并重新申请下载链接
        /// </summary>
        protected void Redownload()
        {
            Cancel();
            Recovery();
        }

        public override event EventHandler DownloadCompleted;

        public override event EventHandler DownloadCanceled;


        public DownloadTask(string storagePath, string name, string targetUUID) : base()
        {
            TargetUUID = targetUUID;
            SavedLocalPath = storagePath;


            Path = storagePath;
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
            Name = name;
            TargetUUID = targetUUID;
            //DownloadCompleted += downloadFileCompleted;
            DownloadCompleted += (sender, e) =>
            {
                try
                {
                    File.Delete(System.IO.Path.Combine(Path, $"{Name}.downloading"));
                }
                catch (IOException ex)
                {
                    ex.Submit();
                }
            };

            WeakEventManager<DispatcherTimer, EventArgs>.AddHandler(ITransferItemViewModel.timer, nameof(ITransferItemViewModel.timer.Tick), Callback);

            void Callback(object sender, EventArgs e)
            {
                if (lastCompletedSize == CompletedBytes)
                {
                    retryTimes++;
                }
                else
                {
                    lastCompletedSize = CompletedBytes;
                }

                if (retryTimes >= 60)
                {
                    retryTimes = 0;
                    Redownload();
                }

                OnPropertyChanged(nameof(Completed));
                OnPropertyChanged(nameof(Speed));
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(Progress));
            }

        }

        public override string ToString()
        {
            Pause(null);
            var record = new DownloadTaskRecord
            {
                LocalPath = SavedLocalPath,
                TargetUUID = TargetUUID,
                Name = Name,
            };
            return JsonConvert.SerializeObject(record);
        }
    }
}
