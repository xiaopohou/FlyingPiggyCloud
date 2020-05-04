using SixTransporter.UploadEngine;
using System;
using System.IO;

namespace SixCloudCore.FileUploader
{
    public static class EzWcs
    {
        private static readonly SliceUploadWorker sliceUploadWorker = new SliceUploadWorker();

        private static readonly SimpleUploadWorker simpleUploadWorker = new SimpleUploadWorker();

        public static IUploadTask NewTask(string filePath, string token, Uri directAddress, Uri partedAddress)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Length < SliceUploadWorker.BLOCKSIZE)
            {
                SimpleUploadTask task = new SimpleUploadTask(filePath, token, partedAddress.AbsoluteUri.Remove(partedAddress.AbsoluteUri.Length - 1));
                simpleUploadWorker.AddTask(task);
                return task;
            }
            else
            {
                SliceUploadTask task = new SliceUploadTask(filePath, token, partedAddress.AbsoluteUri.Remove(partedAddress.AbsoluteUri.Length - 1));
                sliceUploadWorker.AddTask(task);
                return task;
            }
        }
    }
}
