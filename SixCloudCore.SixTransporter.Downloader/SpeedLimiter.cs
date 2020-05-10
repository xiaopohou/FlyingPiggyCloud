using System.Threading;
using Newtonsoft.Json;

namespace SixCloudCore.SixTransporter.Downloader
{
    public class SpeedLimiter
    {
        public long Limit { get; set; }

        [JsonIgnore]
        public long Current { get; private set; }

        [JsonIgnore]
        public bool Running { get; private set; }

        public void Run()
        {
            if (Running || Limit <= 0) return;
            Running = true;
            new Thread(() =>
                {
                    while (Running)
                    {
                        Thread.Sleep(100);
                        if (Current - (Limit / 10) < 0)
                            Current = 0;
                        else
                            Current -= Limit / 10;
                    }
                })
                { IsBackground = true }.Start();
        }

        public void Stop()
        {
            Running = false;
        }

        public void Downloaded(long size)
        {
            if (Running)
            {
                Current += size;
                while (Current > Limit)
                    Thread.Sleep(10);
            }
        }
    }
}
