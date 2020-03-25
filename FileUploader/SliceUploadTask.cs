using System;
using System.IO;

namespace SixCloudCore.FileUploader
{
    internal class SliceUploadTask : IUploadTask
    {
        public string FilePath { get; protected set; }

        public string Token { get; protected set; }

        public string Address { get; protected set; }

        public string Hash { get; internal set; }

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

        public bool TaskOperate(UploadTaskStatus todo)
        {
            //仅接受todo为active/pause/abort，其余指令将被丢弃
            lock (syncUploadTaskStatusObject)
            {
                if (uploadTaskStatus == todo)
                {
                    return true;
                }
                switch (todo)
                {
                    case UploadTaskStatus.Active:
                        if (uploadTaskStatus == UploadTaskStatus.Pause)
                        {
                            uploadTaskStatus = UploadTaskStatus.Active;
                            return true;
                        }
                        else if (uploadTaskStatus == UploadTaskStatus.Error)
                        {
                            UploadBatch = Guid.NewGuid().ToString();
                            TotalBytes = new FileInfo(FilePath).Length;
                            TotalBlockCount = (TotalBytes + SliceUploadWorker.BLOCKSIZE - 1) / SliceUploadWorker.BLOCKSIZE;
                            TotalContents = new string[TotalBlockCount];
                            UploadTaskStatus = UploadTaskStatus.Active;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case UploadTaskStatus.Pause:
                        if (uploadTaskStatus == UploadTaskStatus.Active)
                        {
                            uploadTaskStatus = UploadTaskStatus.Pause;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case UploadTaskStatus.Abort:
                        if (uploadTaskStatus != UploadTaskStatus.Completed)
                        {
                            uploadTaskStatus = UploadTaskStatus.Abort;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    default:
                        return false;
                }
            }
        }
    }
}
