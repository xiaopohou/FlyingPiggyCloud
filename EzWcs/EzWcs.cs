using System.IO;

namespace EzWcs
{
    public static class EzWcs
    {
        private static readonly SliceUploadWorker sliceUploadWorker = new SliceUploadWorker();

        private static readonly SimpleUploadWorker simpleUploadWorker = new SimpleUploadWorker();

        public static IUploadTask NewTask(string filePath, string token, string uploadAddress)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Length < SliceUploadWorker.BLOCKSIZE)
            {
                SimpleUploadTask task = new SimpleUploadTask(filePath, token, uploadAddress);
                simpleUploadWorker.AddTask(task);
                return task;
            }
            else
            {
                SliceUploadTask task = new SliceUploadTask(filePath, token, uploadAddress);
                sliceUploadWorker.AddTask(task);
                return task;
            }
        }
    }
}
