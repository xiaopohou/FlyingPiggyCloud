using FileDownloader;
using QingzhenyunApis.Utils;
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

        public double DownloadProgress => fileDownloader.TotalBytesToReceive == 0 ? 0 : fileDownloader.BytesReceived * 100 / fileDownloader.TotalBytesToReceive;

        public string Completed => Calculators.SizeCalculator(fileDownloader.BytesReceived);

        public string Total => Calculators.SizeCalculator(fileDownloader.TotalBytesToReceive);

        public string Name { get; private set; }

        public string Path { get; }

        public string CurrentFileFullPath => fileDownloader.LocalFileName;

        //public event EventHandler<DownloadFileCompletedArgs> DownloadFileCompleted;

        //public event EventHandler<DownloadFileProgressChangedArgs> DownloadFileProgressChanged;

        public async void Start()
        {
            lock (statusSyncRoot)
            {
                if (Status != TaskStatus.Pause)
                {
                    return;
                }
                Status = TaskStatus.Running;
            }
            await Task.Run(() =>
            {
                fileDownloader.Start();
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
                    Status = TaskStatus.Pause;
                }
            });
        }

        public void Stop()
        {
            //DownloadFileCompleted?.Invoke(this, null);
            lock (statusSyncRoot)
            {
                fileDownloader?.Cancel();
            }
        }

        public DownloadTask(string storagePath, string name, RefreshUri getDownloadUri, EventHandler<DownloadFileCompletedArgs> downloadFileCompleted, EventHandler<DownloadFileProgressChangedArgs> downloadFileProgressChanged)
        {
            Path = storagePath;
            Name = name;

            fileDownloader = new FileDownloadTask(Path, getDownloadUri, name);
            fileDownloader.DownloadFileCompleted += downloadFileCompleted;
            fileDownloader.DownloadProgressChanged += downloadFileProgressChanged;

        }
    }
}
