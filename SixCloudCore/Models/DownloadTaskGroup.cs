using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods.V3;
using SixCloudCore.SixTransporter.Downloader;
using SixCloudCore.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using QingzhenyunApis.Exceptions;

namespace SixCloudCore.Models
{
    /// <summary>
    /// 文件夹下载任务
    /// </summary>
    internal class DownloadTaskGroup : DownloadingTaskViewModel
    {
        private TransferTaskStatus status = TransferTaskStatus.Pause;

        public override string Name { get; protected set; }

        public override string TargetUUID { get; protected set; }

        public override TransferTaskStatus Status => status;
        public override double Progress => CompletedCount * 100 / TotalCount;

        public ConcurrentQueue<DownloadTaskRecord> WaittingTasks { get; } = new ConcurrentQueue<DownloadTaskRecord>();

        public List<HttpDownloader> RunningTasks { get; } = new List<HttpDownloader>(16);

        public List<DownloadTaskRecord> CompletedTasks { get; } = new List<DownloadTaskRecord>();

        public override string CurrentFileFullPath => throw new NotImplementedException();

        public long CompletedCount => CompletedTasks.Count;

        public override string Completed => $"已完成{CompletedCount}";

        public override string SavedLocalPath { get; protected set; }

        public long TotalCount { get; private set; } = 0;

        public override string Total => $"共{TotalCount}个项目";

        public override string Speed { get; }

        public override event EventHandler DownloadCompleted;
        public override event EventHandler DownloadCanceled;

        /// <summary>
        /// 初始化任务组
        /// </summary>
        /// <returns></returns>
        public async Task<DownloadTaskGroup> InitTaskGroup()
        {
            await DownloadHelper(TargetUUID, Path.Combine(SavedLocalPath, Name), 0);
            return this;

            async Task DownloadHelper(string uuid, string localParentPath, int depthIndex)
            {
                await foreach (FileMetaData child in FileListViewModel.CreateFileListEnumerator(0, identity: uuid))
                {
                    if (!child.Directory)
                    {
                        if (!Directory.Exists(localParentPath))
                        {
                            Directory.CreateDirectory(localParentPath);
                        }

                        WaittingTasks.Enqueue(new DownloadTaskRecord
                        {
                            TargetUUID = child.UUID,
                            LocalPath = localParentPath,
                            Name = child.Name
                        });
                        TotalCount++;
                    }
                    else
                    {
                        string nextPath = Path.Combine(localParentPath, child.Name);
                        Directory.CreateDirectory(nextPath);
                        if (depthIndex < 32)
                        {
                            await DownloadHelper(child.UUID, nextPath, depthIndex + 1);
                        }
                        else
                        {
                            ThreadPool.QueueUserWorkItem(async (state) => await DownloadHelper(child.UUID, nextPath, 0), null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 尝试下载等待的任务，每500毫秒触发一次
        /// </summary>
        /// <returns></returns>
        public async Task StartWaittingTasks()
        {
            if (Status != TransferTaskStatus.Running)
            {
                return;
            }

            while (WaittingTasks.TryDequeue(out DownloadTaskRecord task))
            {
                string downloadPath = Path.Combine(task.LocalPath, task.Name);
                string downloadUrl = (await FileSystem.GetDownloadUrlByIdentity(task.TargetUUID)).DownloadAddress;
                lock (RunningTasks)
                {
                    if (RunningTasks.Count >= 16)
                    {
                        //如果有超过16个正在进行的任务，把当前任务塞回去，并终止循环
                        WaittingTasks.Enqueue(task);
                        break;
                    }
                    else
                    {
                        DownloadTaskInfo taskInfo;
                        if (File.Exists(downloadPath + ".downloading"))
                        {
                            taskInfo = DownloadTaskInfo.Load(downloadPath + ".downloading");
                        }
                        else
                        {
                            taskInfo = new DownloadTaskInfo()
                            {
                                DownloadUrl = downloadUrl, // 下载链接，可以为null，任务开始前再赋值初始化
                                DownloadPath = downloadPath,
                                Threads = 4,
                            };
                        }

                        HttpDownloader fileDownloader = new HttpDownloader(taskInfo); // 下载默认会在StartDownload函数初始化, 保存下载进度文件到file.downloading文件
                        fileDownloader.DownloadStatusChangedEvent += (oldValue, newValue, sender) =>
                        {
                            if (newValue == DownloadStatusEnum.Completed)
                            {
                                lock (RunningTasks)
                                {
                                    RunningTasks.Remove(fileDownloader);
                                    CompletedTasks.Add(task);
                                }

                            }
                        };
                        Task.Run(() => fileDownloader.StartDownload());
                    }
                }
            }
        }

        protected override void Cancel(object parameter)
        {
            status = TransferTaskStatus.Stop;
            lock (RunningTasks)
            {
                RunningTasks.ForEach(task =>
                {
                    var filePath = task.StopAndSave().DownloadPath;
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (IOException ex)
                    {
                        ex.ToSentry().AttachTag("TreatedBy", "DownloadTaskGroup").Submit();
                    }

                    try
                    {
                        File.Delete(filePath + ".downloading");
                    }
                    catch (IOException ex)
                    {
                        ex.ToSentry().AttachTag("TreatedBy", "DownloadTaskGroup").Submit();
                    }
                });
                RunningTasks.Clear();

                CompletedTasks.ForEach(task =>
                {
                    try
                    {
                        File.Delete(Path.Combine(task.LocalPath, task.Name));
                    }
                    catch (IOException ex)
                    {
                        ex.ToSentry().AttachTag("TreatedBy", "DownloadTaskGroup").Submit();
                    }
                });
                CompletedTasks.Clear();

                WaittingTasks.Clear();
            }

            DownloadCanceled?.Invoke(this, EventArgs.Empty);
        }

        protected override void Pause(object parameter)
        {
            status = TransferTaskStatus.Pause;
        }

        protected override void Recovery(object parameter)
        {
            status = TransferTaskStatus.Running;
        }

        public DownloadTaskGroup(string targetUUID, string localStoragePath, string name)
        {
            if (FileSystem.GetDetailsByIdentity(TargetUUID).Result.Directory)
            {
                TargetUUID = targetUUID;
                SavedLocalPath = localStoragePath;
                Name = name;
            }
            else
            {
                throw new InvalidOperationException("请求下载的对象不是一个文件夹");
            }

            WeakEventManager<DispatcherTimer, EventArgs>.AddHandler(ITransferItemViewModel.timer, nameof(ITransferItemViewModel.timer.Tick), Callback);

            async void Callback(object sender, EventArgs e)
            {
                await StartWaittingTasks();

                OnPropertyChanged(nameof(Completed));
                OnPropertyChanged(nameof(Speed));
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(Progress));
            }

        }
    }
}