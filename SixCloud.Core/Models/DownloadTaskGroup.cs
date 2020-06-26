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
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace SixCloud.Core.Models
{
    /// <summary>
    /// 文件夹下载任务
    /// </summary>
    public class DownloadTaskGroup : DownloadingTaskViewModel
    {
        private TransferTaskStatus status = TransferTaskStatus.Pause;

        /// <summary>
        /// 尝试下载等待的任务，每500毫秒触发一次
        /// </summary>
        /// <returns></returns>
        private async Task StartWaittingTasks()
        {
            if (Status != TransferTaskStatus.Running)
            {
                return;
            }

            while (WaittingTasks.TryDequeue(out IDownloadTask task))
            {
                string downloadPath = Path.Combine(task.LocalPath, task.Name);
                FileMetaData detail = await FileSystem.GetDownloadUrlByIdentity(task.TargetUUID);
                string downloadUrl = detail.DownloadAddress;

                if (detail.Size == 0)
                {
                    File.Create(downloadPath).Close();
                    CompletedTasks.Add(task);
                    continue;
                }
                else
                {
                    lock (RunningTasks)
                    {
                        if (RunningTasks.Count >= 16 || Status != TransferTaskStatus.Running)
                        {
                            //如果有超过16个正在进行的任务，把当前任务塞回去，并终止循环
                            WaittingTasks.Enqueue(task);
                            break;
                        }
                        else
                        {
                            HttpDownloader fileDownloader = CreateHttpDownloader(downloadPath, downloadUrl, task.TargetUUID);
                            RunningTasks[task] = fileDownloader;
                            fileDownloader.DownloadStatusChangedEvent += (oldValue, newValue, sender) =>
                            {
                                if (newValue == DownloadStatusEnum.Completed)
                                {
                                    lock (RunningTasks)
                                    {
                                        RunningTasks.Remove(task);
                                        CompletedTasks.Add(task);
                                        DownloadTaskRecordStatusChanged?.Invoke(task, EventArgs.Empty);
                                    }
                                }
                            };
                            DownloadTaskRecordStatusChanged?.Invoke(task, EventArgs.Empty);
                            Task.Run(() => fileDownloader.StartDownload());
                        }
                    }
                }
            }

            if (CompletedCount != 0 && CompletedCount == TotalCount)
            {
                status = TransferTaskStatus.Completed;
                DownloadCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 目录名
        /// </summary>
        public override string Name { get; protected set; }

        /// <summary>
        /// 目录IdentityID
        /// </summary>
        public override string TargetUUID { get; protected set; }

        /// <summary>
        /// 总下载状态
        /// </summary>
        public override TransferTaskStatus Status => status;

        /// <summary>
        /// 总进度
        /// </summary>
        public override double Progress => TotalCount == 0 ? 0 : CompletedCount * 100 / TotalCount;

        /// <summary>
        /// 全部任务清单
        /// </summary>
        public List<IDownloadTask> TaskList { get; } = new List<IDownloadTask>();

        /// <summary>
        /// 等待任务队列
        /// </summary>
        public ConcurrentQueue<IDownloadTask> WaittingTasks { get; } = new ConcurrentQueue<IDownloadTask>();

        ///// <summary>
        ///// 下载中任务
        ///// </summary>
        //public Dictionary<DownloadTaskRecord, HttpDownloader> RunningTasks { get; } = new Dictionary<DownloadTaskRecord, HttpDownloader>(16);

        ///// <summary>
        ///// 已完成任务
        ///// </summary>
        //public List<DownloadTask> CompletedTasks { get; } = new List<DownloadTask>();

        /// <summary>
        /// 本地保存路径
        /// </summary>
        public override string CurrentFileFullPath { get; }

        /// <summary>
        /// 已完成计数
        /// </summary>
        public long CompletedCount
        {
            get
            {
                var completed = from task in TaskList
                                where task.Status == TransferTaskStatus.Completed
                                select task;
                return completed.Count();
            }
        }

        public override string Completed => $"已完成{CompletedCount}";

        public override string SavedLocalPath { get; protected set; }

        /// <summary>
        /// 总任务计数
        /// </summary>
        public long TotalCount => TaskList.Count;

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

        public event EventHandler DownloadTaskRecordStatusChanged;

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

                        DownloadTaskRecord newTask = new DownloadTaskRecord
                        {
                            TargetUUID = child.UUID,
                            LocalPath = localParentPath,
                            Name = child.Name
                        };
                        TaskList.Add(newTask);
                        WaittingTasks.Enqueue(newTask);
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
            lock (RunningTasks)
            {
                status = TransferTaskStatus.Pause;
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

                RunningTasks.Keys.ToList().ForEach(task =>
                {
                    RunningTasks.Remove(task);
                    WaittingTasks.Enqueue(task);
                    DownloadTaskRecordStatusChanged?.Invoke(task, EventArgs.Empty);
                });
            }

            RecoveryCommand.OnCanExecutedChanged(this, EventArgs.Empty);
            PauseCommand.OnCanExecutedChanged(this, EventArgs.Empty);

        }

        protected override void Recovery(object parameter)
        {
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
            record.RunningList.ToList().ForEach(task =>
            {
                TaskList.Add(task);
                WaittingTasks.Enqueue(task);
            });
            record.WaittingList.ToList().ForEach(task =>
            {
                TaskList.Add(task);
                WaittingTasks.Enqueue(task);
            });
            record.CompletedList.ToList().ForEach(task =>
            {
                TaskList.Add(task);
                if (File.Exists(Path.Combine(task.LocalPath, task.Name)))
                {
                    CompletedTasks.Add(task);
                }
                else
                {
                    WaittingTasks.Enqueue(task);
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
                OnPropertyChanged(nameof(Status));
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