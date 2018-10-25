using FlyingAria2c.Aria2Lib;
using System;
using System.Threading.Tasks;

namespace FlyingAria2c
{
    public class DownloadTask
    {
        private string Gid;

        public enum TaskAction
        {
            Active,
            Waiting,
            Paused,
            Error,
            Complete,
            Removed
        }

        public TaskAction Status
        {
            get
            {
                switch (status)
                {
                    case "active":
                        return TaskAction.Active;
                    case "waiting":
                        return TaskAction.Waiting;
                    case "paused":
                        return TaskAction.Paused;
                    case "error":
                        return TaskAction.Error;
                    case "complete":
                        return TaskAction.Complete;
                    case "removed":
                        return TaskAction.Removed;
                    default:
                        return TaskAction.Error;
                }
            }
        }
        private string status;

        public double Progress
        {
            get
            {
                if (totalLength == 0)
                {
                    return 0;
                }
                else
                {
                    double i = completedLength * 100 / totalLength;
                    return i;
                }
            }
        }

        private long totalLength = 0;

        private long completedLength = 0;

        public string DownloadSpeed
        {
            get
            {
                double SpeedLong = downloadSpeed;
                if (SpeedLong / 1024 == 0)
                    return Math.Round(SpeedLong, 2).ToString() + "B/S";
                else if (SpeedLong / 1048576 == 0)
                    return Math.Round((SpeedLong / 1024), 2).ToString() + "KB/S";
                else
                    return Math.Round((SpeedLong / 1048578), 2).ToString() + "MB/S";
            }
        }
        private long downloadSpeed = 0;

        public async Task RefreshStatus()
        {
            var x = await Aria2Methords.TellStatus(Downloader.RpcConnection, Gid);
            status = x["status"];
            totalLength = long.Parse(x["totalLength"]);
            completedLength = long.Parse(x["completedLength"]);
            downloadSpeed = long.Parse(x["downloadSpeed"]);
        }

        private async void Create(string DownloadAdress)
        {
            Gid = await Aria2Methords.AddUri(Downloader.RpcConnection, DownloadAdress);
            await RefreshStatus();
        }

        public DownloadTask(string DownloadAdress)
        {
            Create(DownloadAdress);
        }
    }
}