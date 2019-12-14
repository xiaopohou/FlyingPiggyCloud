using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FileDownloader
{
    internal class DownloadPorter
    {
        private const int bufferSize = 1024 * 512;
        private const int speedLimit = 0;
        private readonly byte[] binaryBuffer = new byte[bufferSize];
        private readonly HttpClient httpClient = new HttpClient();


        internal async Task NextJob(ISplittableTask task)
        {
            try
            {
                bool error;
                do
                {
                    try
                    {
                        error = false;
                        await task.AchieveSlice(httpClient, binaryBuffer);
                    }
                    catch (DownloadException)
                    {
                        error = true;
                    }
                    catch (NullReferenceException)
                    {
                        error = true;
                    }
                } while (error);

            }
            finally
            {
                DownloadFactory.Add(this);
            }
        }

    }

    internal interface ISplittableTask
    {

        internal Task AchieveSlice(HttpClient httpClient, byte[] binaryBuffer);

        public bool IsRunning { get; }
    }

    internal static class DownloadFactory
    {
        private static readonly ConcurrentQueue<ISplittableTask> tasks = new ConcurrentQueue<ISplittableTask>();

        private static readonly Timer timer = new Timer(async (_) => await DistributionTask(), null, 0, 1000);

        private static readonly ConcurrentQueue<DownloadPorter> porters = new ConcurrentQueue<DownloadPorter>(new DownloadPorter[] { new DownloadPorter(), new DownloadPorter(), new DownloadPorter(), new DownloadPorter(), new DownloadPorter() });

        private static async Task DistributionTask()
        {
            if (porters.TryDequeue(out DownloadPorter idlePorter))
            {
                if (tasks.TryDequeue(out ISplittableTask outstandingTask))
                {
                    await idlePorter.NextJob(outstandingTask);
                }
                else
                {
                    porters.Enqueue(idlePorter);
                }
            }
        }

        internal static void Add(FileDownloadTask task)
        {
            tasks.Enqueue(task);
        }

        internal static void Add(DownloadPorter porter)
        {
            porters.Enqueue(porter);
        }
    }
}
