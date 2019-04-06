using FileDownloader;
using SixCloud.Controllers;
using System;

namespace SixCloud.Models
{
    internal class DownloadTask
    {
        private IFileDownloader fileDownloader;

        private static Cache Cache = new Cache();

        private readonly Uri downloadResource;

        private readonly string storagePath;

        public double DownloadProgress => fileDownloader.BytesReceived * 100 / fileDownloader.TotalBytesToReceive;

        public string Completed => Calculators.SizeCalculator(fileDownloader.BytesReceived);

        public string Total => Calculators.SizeCalculator(fileDownloader.TotalBytesToReceive);

        public string Name => fileDownloader?.GetLocakFileName();

        public string Path => storagePath.LastIndexOf(@"\") + 1 == storagePath.Length ? storagePath + Name : storagePath + @"\" + Name;

        public event EventHandler<DownloadFileCompletedArgs> DownloadFileCompleted;

        public event EventHandler<DownloadFileProgressChangedArgs> DownloadFileProgressChanged;

        public void Start()
        {
            if(fileDownloader==null)
            {
                fileDownloader = new FileDownloader.FileDownloader(Cache);
                fileDownloader.DownloadFileCompleted += DownloadFileCompleted;
                fileDownloader.DownloadProgressChanged += DownloadFileProgressChanged;
                fileDownloader.DownloadFileAsyncPreserveServerFileName(downloadResource, storagePath);
            }
            else
            {
                fileDownloader.Dispose();
                Start();
            }
        }

        public void Pause()
        {
            if(fileDownloader!=null)
            {
                fileDownloader.Dispose();
            }
        }
        public void Stop()
        {
            if(fileDownloader!=null)
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
