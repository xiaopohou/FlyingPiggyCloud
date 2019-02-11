using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlyingPiggyCloud.Controllers
{
    internal static class MultThreadHelper
    {
        private static Task[] runningTasks = new Task[20];

        private static ConcurrentQueue<Task> waittingTasks = new ConcurrentQueue<Task>();

        private static void Start()
        {
            ThreadPool.QueueUserWorkItem((status) =>
            {
                while(true)
                {
                    Thread.Sleep(10);
                    Check();
#if DEBUG
                    Console.WriteLine($"Current Waitting Tasks is {waittingTasks.Count}");
#endif
                }
            });
        }

        private static void Check()
        {
            List<int> completedIndex = new List<int>();
            for(int i=0; i<runningTasks.Length;i++)
            {
                if(runningTasks[i]==null||runningTasks[i].IsCompleted)
                {
                    completedIndex.Add(i);
                }
            }
            for(int i=0;i<waittingTasks.Count&&i<completedIndex.Count;i++)
            {
                if(waittingTasks.TryDequeue(out runningTasks[completedIndex[i]]))
                {
                    runningTasks[completedIndex[i]].Start();
                }
            }
        }

        public static void NewTask(Task task)
        {
            waittingTasks.Enqueue(task);
        }

        static MultThreadHelper()
        {
            Start();
        }
    }
}
