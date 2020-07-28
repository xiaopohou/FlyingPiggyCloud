using QingzhenyunApis.Exceptions;
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
                    taskManuals.Remove(taskManual);
                else if (taskManuals.FirstOrDefault(x => x.Guid == taskManual.Parent) is DirectoryDownloadTask directoryDownloadTask)
                    directoryDownloadTask.Remove(taskManual);
            }
        }

        private static void TaskLoop(object parameters)
        {
            do
            {
                try
                {
                    //ITaskManual task = EnumerableTask().FirstOrDefault();
                    //task?.Run();
                    foreach (var task in EnumerableTask())
                    {
                        task?.Run();
                    }
                }
                catch (Exception ex)
                {
                    ex.Submit(nameof(TaskLoop));
                }
            } while (true);
        }

        static TaskManual()
        {
            Thread taskLoopThread = new Thread(TaskLoop)
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
            //TaskAdded?.Invoke(taskManual, EventArgs.Empty);
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