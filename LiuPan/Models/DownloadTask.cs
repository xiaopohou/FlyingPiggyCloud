using FileDownloader;
using SixCloud.Controllers;
using System;
using System.Threading.Tasks;

namespace SixCloud.Models
{
    internal class DownloadTask
    {
        public enum TaskStatus
        {
            Running,
            Pause,
            Cancel
        }

        public TaskStatus Status { get; private set; } = TaskStatus.Pause;
        private readonly object statusSyncRoot = new object();

        private IFileDownloader fileDownloader;
        private static readonly TransmissionProgressController.DownloadingCache Cache = new TransmissionProgressController.DownloadingCache();

        private readonly Uri downloadResource;

        private readonly string storagePath;

        public double DownloadProgress => fileDownloader.BytesReceived * 100 / fileDownloader.TotalBytesToReceive;

        public string Completed => Calculators.SizeCalculator(fileDownloader.BytesReceived);

        public string Total => Calculators.SizeCalculator(fileDownloader.TotalBytesToReceive);

        public string Name => System.IO.Path.GetFileName(Path);

        public string Path => fileDownloader?.GetLocalFileName();

        public event EventHandler<DownloadFileCompletedArgs> DownloadFileCompleted;

        public event EventHandler<DownloadFileProgressChangedArgs> DownloadFileProgressChanged;

        public void Start()
        {
            Task.Run(() =>
            {
                lock (statusSyncRoot)
                {
                    if (Status != TaskStatus.Pause)
                    {
                        return;
                    }
                    if (fileDownloader == null)
                    {
                        fileDownloader = new FileDownloader.FileDownloader(Cache);
                        fileDownloader.DownloadFileCompleted += DownloadFileCompleted;
                        fileDownloader.DownloadProgressChanged += DownloadFileProgressChanged;
                    }
                    fileDownloader.DownloadFileAsyncPreserveServerFileName(downloadResource, storagePath);
                    Status = TaskStatus.Running;
                }
            });
        }

        public void Pause()
        {
            Task.Run(() =>
            {
                lock (statusSyncRoot)
                {
                    if (Status != TaskStatus.Running)
                    {
                        return;
                    }
                    fileDownloader.Pause();
                    fileDownloader = null;
                    GC.Collect();
                    Status = TaskStatus.Pause;
                }
            });
        }

        public void Stop()
        {
            lock (statusSyncRoot)
            {
                fileDownloader?.CancelDownloadAsync();
            }
        }

        public DownloadTask(string downloadAdress, string storagePath)
        {
            downloadResource = new Uri(downloadAdress);
            this.storagePath = storagePath;
        }
    }
}
