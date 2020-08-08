using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using SixCloud.Core.ViewModels;
using SixCloudCore.SixTransporter.Downloader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;

namespace SixCloud.Core.Models.Download
{
    public static class TaskManual
    {
        private static readonly List<ITaskManual> taskManuals = new List<ITaskManual>();
        private static bool ApplicationClosing = false;

        /// <summary>
        /// 回收已完成任务
        /// </summary>
        private static void Trim()
        {
            lock (taskManuals)
            {
                taskManuals.Where(x => x.IsCompleted)
                           .ToList()
                           .ForEach(x =>
                           {
                               taskManuals.Remove(x);
                           });
            }
        }

        private static IEnumerable<ITaskManual> EnumerableTask()
        {
            lock (taskManuals)
            {
                var i = 0;
                foreach (var task in taskManuals)
                {
                    if (i < 10)
                    {
                        switch (task)
                        {
                            case CommonFileDownloadTask commonFile:
                                switch (commonFile.Status)
                                {
                                    case DownloadStatusEnum.Waiting:
                                        i++;
                                        yield return commonFile;
                                        break;
                                    case DownloadStatusEnum.Downloading:
                                        i++;
                                        continue;
                                    default:
                                        continue;
                                }

                                break;
                            case EmptyFileDownloadTask emptyFile:
                                if (emptyFile.IsCompleted || emptyFile.Paused)
                                {
                                    continue;
                                }
                                else
                                {
                                    i++;
                                    yield return emptyFile;
                                }

                                break;
                            case DirectoryDownloadTask directory:
                                if (directory.Initialized)
                                {
                                    foreach (var child in directory.Children)
                                    {
                                        if (i < 10)
                                        {
                                            switch (child)
                                            {
                                                case CommonFileDownloadTask commonFile:
                                                    switch (commonFile.Status)
                                                    {
                                                        case DownloadStatusEnum.Waiting:
                                                            i++;
                                                            yield return commonFile;
                                                            break;
                                                        case DownloadStatusEnum.Downloading:
                                                            i++;
                                                            continue;
                                                        default:
                                                            continue;
                                                    }

                                                    break;
                                                case EmptyFileDownloadTask emptyFile:
                                                    if (emptyFile.IsCompleted)
                                                    {
                                                        continue;
                                                    }
                                                    else
                                                    {
                                                        i++;
                                                        yield return emptyFile;
                                                    }

                                                    break;
                                            }

                                        }
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            Trim();
            Thread.Sleep(2000);
        }

        public static void Remove(ITaskManual taskManual)
        {
            lock (taskManuals)
            {
                if (taskManual.Parent == Guid.Empty)
                {
                    taskManuals.Remove(taskManual);
                }
                else if (taskManuals.FirstOrDefault(x => x.Guid == taskManual.Parent) is DirectoryDownloadTask directoryDownloadTask)
                {
                    directoryDownloadTask.Remove(taskManual);
                }
            }
        }

        private static void TaskLoop(object parameters)
        {
            do
            {
                try
                {
                    foreach (var task in EnumerableTask())
                    {
                        task?.Run();
                    }
                }
                catch (Exception ex)
                {
                    ex.Submit();
                }
            } while (!ApplicationClosing);
        }

        static TaskManual()
        {
            var taskLoopThread = new Thread(TaskLoop)
            {
                Priority = ThreadPriority.BelowNormal,
                IsBackground = true
            };
            taskLoopThread.Start();
        }

        public static void Add(ITaskManual taskManual)
        {
            lock (taskManuals)
            {
                WeakEventManager<ITaskManual, EventArgs>.AddHandler(taskManual, nameof(taskManual.TaskComplete), (sender, e) => TransferCompletedListViewModel.NewDownloadedTask(sender as ITaskManual));

                taskManuals.Add(taskManual);
            }
        }

        public static IEnumerable<TaskManualRecord> Save()
        {
            ApplicationClosing = true;
            lock (taskManuals)
            {
                foreach (var task in taskManuals)
                {
                    if (task.IsCompleted)
                    {
                        continue;
                    }
                    else if (task is DirectoryDownloadTask directoryDownloadTask)
                    {
                        foreach (var child in directoryDownloadTask.Children)
                        {
                            if (task.IsCompleted)
                            {
                                continue;
                            }
                            else
                            {
                                yield return child.ToRecord();
                            }
                        }
                    }

                    yield return task.ToRecord();
                }
            }
        }

        public static void Load(IEnumerable<ITaskManual> manuals)
        {
            var taskGroups = from task in manuals
                             group task by task.Parent into taskGroup
                             orderby taskGroup.Key ascending
                             select taskGroup;

            if (taskGroups.Where(x => x.Key == Guid.Empty).Any())
            {
                var tasks = new List<ITaskManual>();
                foreach (var taskGroup in taskGroups)
                {
                    if (taskGroup.Key == Guid.Empty)
                    {
                        tasks.AddRange(from task in taskGroup let manual = LoadManuals(task) where manual != null select manual);
                    }
                    else
                    {
                        (tasks.First(x => x.Guid == taskGroup.Key) as DirectoryDownloadTask).AddRange(from task in taskGroup let manual = LoadManuals(task) where manual != null select manual);
                    }
                }

                tasks.ForEach(x => TransferListViewModel.AddDownloadingItem(x));
            }

            static ITaskManual LoadManuals(ITaskManual taskManual)
            {
                ITaskManual x;
                try
                {
                    var detail = FileSystem.GetDetailsByIdentity(taskManual.TargetUUID).Result;
                    if (detail.Directory)
                    {
                        x = new DirectoryDownloadTask(taskManual);
                    }
                    else if (detail.Size == 0)
                    {
                        x = new EmptyFileDownloadTask(taskManual);
                    }
                    else
                    {
                        x = CommonFileDownloadTask.Create(taskManual);

                    }

                }
                catch (RequestFailedException ex) when (ex.Code == "FILE_NOT_FOUND")
                {
                    x = null;
                }
                return x;
            }
        }

        public static ObservableCollection<DownloadTaskViewModel> ToObservableCollection()
        {
            lock (taskManuals)
            {
                var list = new ObservableCollection<DownloadTaskViewModel>();
                taskManuals
                    .Where(x => !x.IsCompleted)
                    .ToList()
                    .ForEach(x =>
                    {
                        lock (x)
                        {
                            var taskVM = new DownloadTaskViewModel(x);
                            taskVM.TaskComplete += (sender, e) =>
                            {
                                list.Remove(taskVM);
                            };

                            taskVM.TaskCancel += (sender, e) =>
                            {
                                list.Remove(taskVM);
                            };

                            list.Add(taskVM);
                        }
                    });
                return list;
            }
        }
    }
}