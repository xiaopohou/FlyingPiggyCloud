using FileDownloader;
using SixCloud.Controllers;
using System;

namespace SixCloud.Models
{
    internal class DownloadTask
    {
        private readonly IFileDownloader fileDownloader;

        public double DownloadProgress => fileDownloader.BytesReceived * 100 / fileDownloader.TotalBytesToReceive;

        public string Completed => Calculators.SizeCalculator(fileDownloader.BytesReceived);

        public string Total => Calculators.SizeCalculator(fileDownloader.TotalBytesToReceive);

        public string Name { get; private set; }

        public string Path { get; private set; }

        public event EventHandler<DownloadFileCompletedArgs> OnDownloadFileCompleted;

        public event EventHandler<DownloadFileProgressChangedArgs> OnDownloadFileProgressChanged;

        //public void Pause()=>fileDownloader.

        public DownloadTask(string downloadAdress,string storagePath)
        {
            fileDownloader = new FileDownloader.FileDownloader();
            fileDownloader.DownloadFileAsyncPreserveServerFileName(new Uri(downloadAdress), storagePath);

        }
    }
}
