﻿using Newtonsoft.Json;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using QingzhenyunApis.Utils;
using SixCloudCore.SixTransporter.Downloader;
using SixCloud.Core.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SixCloud.Core.Models
{
    internal class DownloadTask : DownloadingTaskViewModel
    {
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
            DownloadStatusEnum.Failed => TransferTaskStatus.Failed,
            DownloadStatusEnum.Completed => TransferTaskStatus.Completed,
            _ => throw new InvalidCastException()
        };

        public override string Speed => Calculators.SizeCalculator(fileDownloader?.Speed ?? 0) + "/秒";

        public long CompletedBytes => fileDownloader?.Info.DownloadedSize ?? 0;

        protected override async void Recovery(object parameter = null)
        {
            if (fileDownloader?.Status == DownloadStatusEnum.Downloading || fileDownloader?.Status == DownloadStatusEnum.Failed)
            {
                return;
            }

            if (Url == null)
            {
                Url = (await FileSystem.GetDownloadUrlByIdentity(TargetUUID)).DownloadAddress;
            }

            if (fileDownloader == null)
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

                fileDownloader.DownloadStatusChangedEvent += async (oldValue, newValue, sender) =>
                {
                    if (newValue == DownloadStatusEnum.Completed)
                    {
                        DownloadCompleted?.Invoke(sender, null);
                    }
                    else if (newValue == DownloadStatusEnum.Failed)
                    {
                        Thread.Sleep(TimeSpan.FromMinutes(1));
                        Url = (await FileSystem.GetDownloadUrlByIdentity(TargetUUID)).DownloadAddress;
                        taskInfo.DownloadUrl = Url;
                        await Task.Run(() => fileDownloader?.StartDownload());
                    }
                    OnPropertyChanged(nameof(Status));
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
            fileDownloader.AllFileStreamDisposed += (sender, e) =>
            {
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
            };

            fileDownloader.StopAndSave(true)?.Save(System.IO.Path.Combine(Path, $"{Name}.downloading"));
            fileDownloader = null;
            DownloadCanceled?.Invoke(this, EventArgs.Empty);
        }

        ///// <summary>
        ///// 删除本地文件并重新申请下载链接
        ///// </summary>
        //protected void Redownload()
        //{
        //    Cancel();
        //    Recovery();
        //}

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
                //if (lastCompletedSize == CompletedBytes)
                //{
                //    retryTimes++;
                //}
                //else
                //{
                //    retryTimes = 0;
                //    lastCompletedSize = CompletedBytes;
                //}

                //if (retryTimes >= 60)
                //{
                //    retryTimes = 0;
                //    Redownload();
                //}

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