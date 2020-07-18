using QingzhenyunApis.Exceptions;
using SixCloudCore.SixTransporter.Downloader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace SixCloud.Core.Models.Download
{
    public static class TaskManual
    {
        public static ObservableCollection<ITaskManual> TaskManuals { get; } = new ObservableCollection<ITaskManual>();

        private static IEnumerable<ITaskManual> EnumerableTask()
        {
            for (int index = 0; index < 10; index++)
            {
                var task = TaskManuals.FirstOrDefault(x => x is CommonFileDownloadTask commonFile && (commonFile.Status == DownloadStatusEnum.Waiting || commonFile.Status == DownloadStatusEnum.Downloading));
                if (task == default)
                {
                    Thread.Sleep(1000);
                    yield break;
                }
                else if ((task as CommonFileDownloadTask).Status == DownloadStatusEnum.Waiting)
                {
                    yield return task;
                }
            }
        }

        private static void TaskLoop(object parameters)
        {
            do
            {
                try
                {
                    ITaskManual task = EnumerableTask().FirstOrDefault();
                    task?.Run();
                }
                catch (Exception ex)
                {
                    ex.Submit(nameof(TaskLoop));
                }
            } while (true);
        }

        static TaskManual()
        {
            Thread taskLoopThread = new Thread(TaskLoop);
            taskLoopThread.Priority = ThreadPriority.BelowNormal;
            taskLoopThread.Start();
        }
    }
}