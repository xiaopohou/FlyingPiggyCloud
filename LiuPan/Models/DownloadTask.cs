using FileDownloader;
using SixCloud.Controllers;
using System;
using System.Threading.Tasks;

namespace SixCloud.Models
{
    internal class DownloadTask
    {
        private IFileDownloader fileDownloader;

        private static readonly Cache Cache = new Cache();

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
                if (fileDownloader == null)
                {
                    fileDownloader = new FileDownloader.FileDownloader(Cache);
                    fileDownloader.DownloadFileCompleted += DownloadFileCompleted;
                    fileDownloader.DownloadProgressChanged += DownloadFileProgressChanged;
                }
                fileDownloader.DownloadFileAsyncPreserveServerFileName(downloadResource, storagePath);
            });
        }

        public void Pause()
        {
            Task.Run(() =>
            {
                fileDownloader.Pause();
                fileDownloader = null;
                GC.Collect();
            });
        }

        public void Stop()
        {
            if (fileDownloader != null)
            {
                fileDownloader.CancelDownloadAsync();
            }
        }

        public DownloadTask(string downloadAdress, string storagePath)
        {
            downloadResource = new Uri(downloadAdress);
            this.storagePath = storagePath;
        }
    }
}
