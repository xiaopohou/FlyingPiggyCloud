using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileDownloader
{
    internal class DownloadPorter
    {
        private const int bufferSize = 1024 * 512;
        private const int speedLimit = 0;
        private ISplittableTask currentTask;
        private readonly byte[] binaryBuffer = new byte[bufferSize];
        HttpClient httpClient = new HttpClient();

        internal bool Idle => currentTask == null;

        internal void NextJob(ISplittableTask task)
        {
            try
            {
                task.CurrentWorker = this;
                currentTask = task;
                task.AchieveSlice(httpClient, binaryBuffer);
            }
#warning 此处应捕捉更明确的异常类型
            catch (Exception)
            {

            }
            finally
            {
                task.CurrentWorker = null;
                currentTask = null;
            }

        }

    }

    internal interface ISplittableTask
    {
        internal DownloadPorter CurrentWorker { get; set; }

        internal Task AchieveSlice(HttpClient httpClient, byte[] binaryBuffer);

        public bool IsRunning { get; }
    }

    internal static class DownloadFactory
    {
        private static readonly List<ISplittableTask> tasks = new List<ISplittableTask>(8);

        private static readonly Timer timer;

        private static readonly DownloadPorter[] porters = new DownloadPorter[] { new DownloadPorter(), new DownloadPorter(), new DownloadPorter(), new DownloadPorter(), new DownloadPorter() };

        private static void DistributionTask()
        {
            var idlePorter = porters.FirstOrDefault(x => x.Idle);
            var outstandingTask = tasks.FirstOrDefault(x => x.CurrentWorker == null);
            if (idlePorter == default || outstandingTask == default)
            {
                return;
            }
            else
            {
                idlePorter.NextJob(outstandingTask);
            }
        }

        internal static void Add(FileDownloadTask task)
        {
            if (tasks.Contains(task))
            {
                return;
            }
            else
            {
                task.DownloadFileCompleted += (sender, e) =>
                {
                    tasks.Remove(sender as ISplittableTask);
                };
                tasks.Add(task);
            }
        }

        static DownloadFactory()
        {
            //ThreadPool.QueueUserWorkItem(_ =>
            //{
            //    while (true)
            //    {
            //        Task.Run(() => DistributionTask());
            //        Thread.Sleep(1000);
            //    }

            //});
            timer = new Timer((_) => DistributionTask(), null, 0, 1000);
        }
    }
}
