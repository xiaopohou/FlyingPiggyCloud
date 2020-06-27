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
    public class DownloadTaskGroup : DownloadingTaskViewModel
    {
        private TransferTaskStatus status = TransferTaskStatus.Pause;
        private readonly DownloadTaskGroupRecord downloadTaskGroupRecord;

        /// <summary>
        /// 尝试下载等待的任务，每500毫秒触发一次
        /// </summary>
        /// <returns></returns>
        private void StartWaittingTasks()
        {
            if (Status != TransferTaskStatus.Running)
            {
                return;
            }

            lock (TaskList)
            {
                while ((from taskInfo in TaskList where taskInfo.Status == TransferTaskStatus.Running select taskInfo).Count() < 16 && WaittingTasks.TryDequeue(out IDownloadTask task))
                {
                    task.RecoveryCommand.Execute(null);
                }

                IEnumerable<IDownloadTask> errorTasks = from taskInfo in TaskList
                                                        where taskInfo.Status != TransferTaskStatus.Running && taskInfo.Status != TransferTaskStatus.Completed
                                                        where !WaittingTasks.Contains(taskInfo)
                                                        select taskInfo;

                if (errorTasks.Any())
                {
                    errorTasks.ToList().ForEach(x => WaittingTasks.Enqueue(x));
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
                IEnumerable<IDownloadTask> completed = from task in TaskList
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

        public override string FriendlySpeed
        {
            get
            {
                long speedTemp = 0;
                IEnumerable<long> runnings = from task in TaskList
                                             where task.Status == TransferTaskStatus.Running
                                             select task.Speed;
                runnings.ToList().ForEach(speed => speedTemp += speed);
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
            WeakEventManager<DispatcherTimer, EventArgs>.AddHandler(ITransferItemViewModel.timer, nameof(ITransferItemViewModel.timer.Tick), TimerCallBack);

            //当前进程创建的任务组
            if (downloadTaskGroupRecord == null)
            {
                await DownloadHelper(TargetUUID, Path.Combine(SavedLocalPath, Name), 0);
            }
            //从记录文件恢复的任务组
            else
            {
                var waittingTaskList = from task in downloadTaskGroupRecord.TaskList
                                       where downloadTaskGroupRecord.CompletedList.FirstOrDefault(x => x.TargetUUID == task.TargetUUID) == default
                                       select task;
                if (waittingTaskList.Any())
                {
                    foreach (var waittingTask in waittingTaskList)
                    {
                        var detail = await FileSystem.GetDetailsByIdentity(waittingTask.TargetUUID);
                        if (detail.Size == 0)
                        {
                            TaskList.Add(new EmptyFileDownloadTask(waittingTask.SavedLocalPath, waittingTask.Name, waittingTask.TargetUUID));
                        }
                        else
                        {
                            TaskList.Add(new DownloadTask(waittingTask.SavedLocalPath, waittingTask.Name, waittingTask.TargetUUID));
                        }
                    }
                }

                if (downloadTaskGroupRecord.CompletedList.Any())
                {
                    foreach (var completedTask in downloadTaskGroupRecord.CompletedList)
                    {
                        TaskList.Add(new CompletedDownloadTask(completedTask.LocalPath, completedTask.Name, completedTask.TargetUUID));
                    }
                }
            }

            return this;

            void TimerCallBack(object sender, EventArgs e)
            {
                StartWaittingTasks();

                OnPropertyChanged(nameof(Completed));
                OnPropertyChanged(nameof(FriendlySpeed));
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(Status));
            }

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

                        FileMetaData detail = await FileSystem.GetDetailsByIdentity(child.UUID);
                        IDownloadTask newTask;
                        if (detail.Size == 0)
                        {
                            newTask = new EmptyFileDownloadTask(localParentPath, child.Name, child.UUID);
                        }
                        else
                        {
                            newTask = new DownloadTask(localParentPath, child.Name, child.UUID);
                        }
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
            lock (TaskList)
            {
                TaskList.ForEach(task =>
                {
                    if (task is DownloadingTaskViewModel downloadingTask)
                    {
                        downloadingTask.CancelCommand.Execute(null);
                    }
                    else if (task is CompletedDownloadTask downloadedTask)
                    {
                        if (File.Exists(task.CurrentFileFullPath))
                        {
                            File.Delete(task.CurrentFileFullPath);
                        }
                    }
                    WaittingTasks.Clear();
                });
            }

            DownloadCanceled?.Invoke(this, EventArgs.Empty);
        }

        protected override void Pause(object parameter)
        {
            lock (TaskList)
            {
                status = TransferTaskStatus.Pause;
                TaskList.ForEach(task =>
                {
                    if (task is DownloadingTaskViewModel downloadingTask && task.Status == TransferTaskStatus.Running)
                    {
                        downloadingTask.PauseCommand.Execute(null);
                        WaittingTasks.Enqueue(task);
                        DownloadTaskRecordStatusChanged?.Invoke(task, EventArgs.Empty);
                    }
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
        }

        public DownloadTaskGroup(DownloadTaskGroupRecord record) : this(record.TargetUUID, record.LocalPath, record.Name)
        {
            downloadTaskGroupRecord = record;
        }

        public override string ToString()
        {
            Pause(null);
            var completed = from task in TaskList
                            where task.Status == TransferTaskStatus.Completed
                            select new DownloadTaskRecord { LocalPath = task.SavedLocalPath, TargetUUID = task.TargetUUID, Name = task.Name };

            DownloadTaskGroupRecord record = new DownloadTaskGroupRecord
            {
                Name = Name,
                TargetUUID = TargetUUID,
                LocalPath = SavedLocalPath,
                CompletedList = completed.ToArray(),
                TaskList = TaskList,
            };
            return JsonConvert.SerializeObject(record);
        }
    }
}