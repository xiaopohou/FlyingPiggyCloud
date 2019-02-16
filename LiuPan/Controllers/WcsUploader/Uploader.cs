using LiuPan.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace LiuPan.Controllers.WcsUploader
{
    public class Uploader
    {
        private const int MaxUploadThreadCount = 10;

        private const int CheckFrequency = 100;

        private static IUploadTask[] runningTasks = new IUploadTask[MaxUploadThreadCount];

        private static ConcurrentQueue<IUploadTask> waittingTasks = new ConcurrentQueue<IUploadTask>();

        private static readonly bool isCheck = true;

        private static void Start()
        {
            ThreadPool.QueueUserWorkItem((status) =>
            {
                while (isCheck)
                {
                    Thread.Sleep(CheckFrequency);
                    Check();
                }
            });
        }

        private static void Check()
        {
            List<int> reassignedTaskIndex = new List<int>();
            for (int i = 0; i < runningTasks.Length; i++)
            {
                if (runningTasks[i] == null || runningTasks[i].IsRunning)
                {
                    reassignedTaskIndex.Add(i);
                }
            }
            for (int i = 0; i < waittingTasks.Count && i < reassignedTaskIndex.Count; i++)
            {
                if (waittingTasks.TryDequeue(out runningTasks[reassignedTaskIndex[i]]))
                {
                    runningTasks[reassignedTaskIndex[i]].Start();
                }
            }
        }

        public static void NewTask(IUploadTask task)
        {
            waittingTasks.Enqueue(task);
        }

        static Uploader()
        {
            Start();
        }
    }
}
