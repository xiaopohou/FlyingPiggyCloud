using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using QingzhenyunApis.Utils;
using SixCloud.Core.ViewModels;
using SixCloudCore.SixTransporter.Downloader;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SixCloud.Core.Models
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
        public override double Progress => TotalCount == 0 ? 0 : CompletedCount * 100 / TotalCount;

        public ConcurrentQueue<DownloadTaskRecord> WaittingTasks { get; } = new ConcurrentQueue<DownloadTaskRecord>();

        public Dictionary<DownloadTaskRecord, HttpDownloader> RunningTasks { get; } = new Dictionary<DownloadTaskRecord, HttpDownloader>(16);

        public List<DownloadTaskRecord> CompletedTasks { get; } = new List<DownloadTaskRecord>();

        public override string CurrentFileFullPath { get; }

        public long CompletedCount => CompletedTasks.Count;

        public override string Completed => $"已完成{CompletedCount}";

        public override string SavedLocalPath { get; protected set; }

        public long TotalCount { get; private set; } = 0;

        public override string Total => $"{TotalCount}个项目";

        public override string Speed
        {
            get
            {
                long speedTemp = 0;
                RunningTasks.Values.ToList().ForEach(task => speedTemp += task.Speed);
                return Calculators.SizeCalculator(speedTemp) + "/秒";
            }
        }

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
                        RunningTasks[task] = fileDownloader;
                        fileDownloader.DownloadStatusChangedEvent += (oldValue, newValue, sender) =>
                        {
                            if (newValue == DownloadStatusEnum.Completed)
                            {
                                lock (RunningTasks)
                                {
                                    RunningTasks.Remove(task);
                                    CompletedTasks.Add(task);
                                }

                            }
                        };
                        fileDownloader.DownloadStatusChangedEvent += DownloadFailedEventHandler;
                        Task.Run(() => fileDownloader.StartDownload());
                    }
                }
            }

            if (CompletedCount != 0 && CompletedCount == TotalCount)
            {
                status = TransferTaskStatus.Completed;
                DownloadCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void Cancel(object parameter)
        {
            status = TransferTaskStatus.Stop;
            lock (RunningTasks)
            {
                RunningTasks.Values.ToList().ForEach(task =>
                {
                    task.AllFileStreamDisposed += (sender, e) =>
                    {
                        string filePath = (sender as HttpDownloader).Info.DownloadPath;
                        try
                        {
                            File.Delete(filePath);
                        }
                        catch (IOException ex)
                        {
                            ex.ToSentry().TreatedBy(nameof(DownloadTaskGroup)).Submit();
                        }
                        try
                        {
                            File.Delete(filePath + ".downloading");
                        }
                        catch (IOException ex)
                        {
                            ex.ToSentry().TreatedBy(nameof(DownloadTaskGroup)).Submit();
                        }
                    };
                    task.StopAndSave(true);
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
                        ex.ToSentry().TreatedBy(nameof(DownloadTaskGroup)).Submit();
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
            lock (RunningTasks)
            {
                RunningTasks.Values.ToList().ForEach(task =>
                {
                    try
                    {
                        task.StopAndSave()?.Save(task.Info.DownloadPath + ".downloading");
                    }
                    catch (NullReferenceException ex)
                    {
                        ex.ToSentry().AttachExtraInfo(nameof(DownloadTask), this).Submit();
                    }
                });
            }

            RecoveryCommand.OnCanExecutedChanged(this, EventArgs.Empty);
            PauseCommand.OnCanExecutedChanged(this, EventArgs.Empty);

        }

        protected override async void Recovery(object parameter)
        {
            foreach (KeyValuePair<DownloadTaskRecord, HttpDownloader> task in RunningTasks)
            {
                if (task.Value?.Status == DownloadStatusEnum.Downloading || task.Value?.Status == DownloadStatusEnum.Failed)
                {
                    return;
                }

                string url = (await FileSystem.GetDownloadUrlByIdentity(TargetUUID)).DownloadAddress;

                if (task.Value == null)
                {
                    string downloadPath = Path.Combine(task.Key.LocalPath, task.Key.Name);

                    DownloadTaskInfo taskInfo;

                    if (File.Exists(downloadPath + ".downloading"))
                    {
                        taskInfo = DownloadTaskInfo.Load(downloadPath + ".downloading");
                    }
                    else
                    {
                        taskInfo = new DownloadTaskInfo()
                        {
                            DownloadUrl = url, // 下载链接，可以为null，任务开始前再赋值初始化
                            DownloadPath = downloadPath,
                            Threads = 4,
                        };
                    }

                    RunningTasks[task.Key] = new HttpDownloader(taskInfo); // 下载默认会在StartDownload函数初始化, 保存下载进度文件到file.downloading文件
                    task.Value.DownloadStatusChangedEvent += (oldValue, newValue, sender) =>
                    {
                        if (newValue == DownloadStatusEnum.Completed)
                        {
                            lock (RunningTasks)
                            {
                                RunningTasks.Remove(task.Key);
                                CompletedTasks.Add(task.Key);
                            }
                        }
                    };
                    task.Value.DownloadStatusChangedEvent += DownloadFailedEventHandler;
                }

                await Task.Run(() => task.Value?.StartDownload());
            }


            status = TransferTaskStatus.Running;
            RecoveryCommand.OnCanExecutedChanged(this, EventArgs.Empty);
            PauseCommand.OnCanExecutedChanged(this, EventArgs.Empty);
        }

        public DownloadTaskGroup(string targetUUID, string localStoragePath, string name)
        {
            TargetUUID = targetUUID;
            SavedLocalPath = localStoragePath;
            Name = name;

            WeakEventManager<DispatcherTimer, EventArgs>.AddHandler(ITransferItemViewModel.timer, nameof(ITransferItemViewModel.timer.Tick), Callback);

            async void Callback(object sender, EventArgs e)
            {
                await StartWaittingTasks();

                OnPropertyChanged(nameof(Completed));
                OnPropertyChanged(nameof(Speed));
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(Status));
            }

        }

        public DownloadTaskGroup(DownloadTaskGroupRecord record)
        {
            TargetUUID = record.TargetUUID;
            SavedLocalPath = record.LocalPath;
            Name = record.Name;
            TotalCount = record.WaittingList.Count + record.RunningList.Count + record.CompletedList.Count;
            record.WaittingList.ToList().ForEach(tasks => WaittingTasks.Enqueue(tasks));
            CompletedTasks.AddRange(record.CompletedList);
            record.RunningList.ToList().ForEach(async (task) =>
            {
                string downloadPath = Path.Combine(task.LocalPath, task.Name);
                string downloadUrl = (await FileSystem.GetDownloadUrlByIdentity(task.TargetUUID)).DownloadAddress;
                lock (RunningTasks)
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
                    RunningTasks[task] = fileDownloader;
                    fileDownloader.DownloadStatusChangedEvent += (oldValue, newValue, sender) =>
                    {
                        if (newValue == DownloadStatusEnum.Completed)
                        {
                            lock (RunningTasks)
                            {
                                RunningTasks.Remove(task);
                                CompletedTasks.Add(task);
                            }
                        }
                    };
                    fileDownloader.DownloadStatusChangedEvent += DownloadFailedEventHandler;
                    Task.Run(() => fileDownloader.StartDownload());
                }
            });

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

        private async void DownloadFailedEventHandler(DownloadStatusEnum oldValue, DownloadStatusEnum newValue, HttpDownloader sender)
        {
            if (newValue == DownloadStatusEnum.Failed)
            {
                Thread.Sleep(TimeSpan.FromMinutes(1));
                try
                {
                    KeyValuePair<DownloadTaskRecord, HttpDownloader> target = RunningTasks.First(x => x.Value == sender);
                    sender.Info.DownloadUrl = (await FileSystem.GetDownloadUrlByIdentity(target.Key.TargetUUID)).DownloadAddress;
                    await Task.Run(() => sender?.StartDownload());
                }
                catch (InvalidOperationException ex)
                {
                    ex.ToSentry().Submit();
                }
            }
        }

        public override string ToString()
        {
            Pause(null);

            DownloadTaskGroupRecord record = new DownloadTaskGroupRecord
            {
                Name = Name,
                TargetUUID = TargetUUID,
                LocalPath = SavedLocalPath,
                WaittingList = WaittingTasks.ToArray(),
                CompletedList = CompletedTasks.ToArray(),
                RunningList = RunningTasks.Keys.ToArray()
            };
            return JsonConvert.SerializeObject(record);
        }
    }
}