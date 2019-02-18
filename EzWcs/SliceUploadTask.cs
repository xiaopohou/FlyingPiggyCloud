using System;
using System.IO;

namespace EzWcs
{
    internal class SliceUploadTask : IUploadTask
    {
        public string FilePath { get; protected set; }

        public string Token { get; protected set; }

        public string Address { get; protected set; }

        public long CompletedBytes
        {
            get
            {
                long completedBytes = CompletedBlockCount * SliceUploadWorker.BLOCKSIZE;
                if (completedBytes > TotalBytes)
                {
                    return TotalBytes;
                }
                else
                {
                    return completedBytes;
                }
            }
        }
        public long CompletedBlockCount
        {
            get
            {
                lock (syncCompletedBlockCountObject)
                {
                    return completedBlockCount;
                }
            }
            set
            {
                lock (syncCompletedBlockCountObject)
                {
                    completedBlockCount = value;
                }
            }
        }
        private long completedBlockCount = 0;
        private readonly object syncCompletedBlockCountObject = new object();

        public long TotalBlockCount { get; internal set; }

        public long TotalBytes { get; protected set; }

        public string[] TotalContents { get; protected set; }

        //public string UploadUrl { get; protected set; }

        public string UploadBatch { get; protected set; }

        public UploadTaskStatus UploadTaskStatus
        {
            get
            {
                lock (syncUploadTaskStatusObject)
                {
                    return uploadTaskStatus;
                }
            }
            set
            {
                lock (syncUploadTaskStatusObject)
                {
                    if (uploadTaskStatus != UploadTaskStatus.Completed)
                    {
                        uploadTaskStatus = value;
                    }
                }
            }
        }
        private UploadTaskStatus uploadTaskStatus;
        private readonly object syncUploadTaskStatusObject = new object();

        public SliceUploadTask(string filePath, string token, string uploadUrl)
        {
            FilePath = filePath;
            Token = token;
            Address = uploadUrl;
            UploadBatch = Guid.NewGuid().ToString();
            TotalBytes = new FileInfo(filePath).Length;
            TotalBlockCount = (TotalBytes + SliceUploadWorker.BLOCKSIZE - 1) / SliceUploadWorker.BLOCKSIZE;
            TotalContents = new string[TotalBlockCount];
            UploadTaskStatus = UploadTaskStatus.Active;
        }
    }
}
