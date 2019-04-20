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

        public string DownloadAddress => downloadResource.ToString();

        public double DownloadProgress => fileDownloader.BytesReceived * 100 / fileDownloader.TotalBytesToReceive;

        public string Completed => Calculators.SizeCalculator(fileDownloader.BytesReceived);

        public string Total => Calculators.SizeCalculator(fileDownloader.TotalBytesToReceive);

        public string Name { get; private set; }

        public string Path { get; }

        public string CurrentFileFullPath => fileDownloader.GetLocalFileName();

        public event EventHandler<DownloadFileCompletedArgs> DownloadFileCompleted;

        public event EventHandler<DownloadFileProgressChangedArgs> DownloadFileProgressChanged;

        public async Task Start()
        {
            await Task.Run(() =>
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
                    fileDownloader.DownloadFileAsyncPreserveServerFileName(downloadResource, Path);
                    Status = TaskStatus.Running;
                }
            });
        }

        public async Task Pause()
        {
            await Task.Run(() =>
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
            DownloadFileCompleted?.Invoke(this, null);
            lock (statusSyncRoot)
            {
                fileDownloader?.CancelDownloadAsync();
            }
        }

        public DownloadTask(string downloadAddress, string storagePath, string name)
        {
            downloadResource = new Uri(downloadAddress);
            Path = storagePath;
            Name = name;
        }
    }
}
