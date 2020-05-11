using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using QingzhenyunApis.Utils;
using SixCloudCore.SixTransporter.Downloader;
using SixCloudCore.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SixCloudCore.Models
{
    internal class DownloadTask
    {
        public TransferTaskStatus Status => (fileDownloader?.Status ?? DownloadStatusEnum.Waiting) switch
        {
            DownloadStatusEnum.Downloading => TransferTaskStatus.Running,
            DownloadStatusEnum.Waiting => TransferTaskStatus.Running,

            DownloadStatusEnum.Paused => TransferTaskStatus.Pause,
            DownloadStatusEnum.Failed => TransferTaskStatus.Pause,
            DownloadStatusEnum.Completed => TransferTaskStatus.Completed,
            _ => throw new InvalidCastException()
        };

        private HttpDownloader fileDownloader;

        public float DownloadProgress => fileDownloader?.DownloadPercentage ?? 0;

        public string Completed => Calculators.SizeCalculator(fileDownloader?.Info.DownloadedSize ?? 0);

        public string Total => Calculators.SizeCalculator(fileDownloader?.Info.ContentSize ?? 0);

        public string Name { get; private set; }
        public string TargetUUID { get; }
        public string Url { get; private set; }
        public string Path { get; }

        public string Speed => Calculators.SizeCalculator(fileDownloader?.Speed ?? 0) + "/秒";

        public string CurrentFileFullPath => fileDownloader?.Info.DownloadPath;

        public long CompletedBytes => fileDownloader?.Info.DownloadedSize ?? 0;

        public async void Start()
        {
            if (fileDownloader?.Status == DownloadStatusEnum.Downloading)
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
                fileDownloader.DownloadStatusChangedEvent += (oldValue, newValue, sender) =>
                {
                    if (newValue == DownloadStatusEnum.Completed)
                    {
                        DownloadCompleted?.Invoke(sender, null);
                    }
                };
            }

            await Task.Run(() => fileDownloader?.StartDownload());
        }

        public void Pause()
        {
            if (Status != TransferTaskStatus.Running)
            {
                return;
            }

            try
            {
                fileDownloader.StopAndSave().Save(System.IO.Path.Combine(Path, $"{Name}.downloading"));
            }
            catch (NullReferenceException ex)
            {
                ex.ToSentry().AttachExtraInfo(nameof(DownloadTask), this).Submit();
            }
        }

        public void Stop()
        {
            fileDownloader.StopAndSave().Save(System.IO.Path.Combine(Path, $"{Name}.downloading"));
            try
            {
                File.Delete(System.IO.Path.Combine(Path, Name));
                File.Delete(System.IO.Path.Combine(Path, $"{Name}.downloading"));
            }
            catch (IOException ex)
            {
                ex.Submit();
            }
        }

        public event EventHandler DownloadCompleted;

        public DownloadTask(string storagePath, string name, string targetUUID, EventHandler downloadFileCompleted)
        {
            Path = storagePath;
            Name = name;
            TargetUUID = targetUUID;
            DownloadCompleted += downloadFileCompleted;
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
        }
    }
}
