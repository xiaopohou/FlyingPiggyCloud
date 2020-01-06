using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace FileDownloader
{
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
                    try
                    {
                        await idlePorter.NextJob(outstandingTask);
                    }
                    catch(NullReferenceException)
                    {
                        tasks.Enqueue(outstandingTask);
                    }
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
