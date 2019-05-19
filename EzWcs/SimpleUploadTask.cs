using System.IO;

namespace EzWcs
{
    internal class SimpleUploadTask : IUploadTask
    {
        public string FilePath { get; protected set; }

        public string Token { get; protected set; }

        public string Hash { get; internal set; }

        public string Address { get; protected set; }

        public long CompletedBytes
        {
            get
            {
                if (UploadTaskStatus == UploadTaskStatus.Completed)
                {
                    return TotalBytes;
                }
                else
                {
                    return 0;
                }
            }
        }

        public long TotalBytes { get; protected set; }

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

        public SimpleUploadTask(string filePath, string token, string uploadUrl)
        {
            FilePath = filePath;
            Token = token;
            Address = uploadUrl;
            TotalBytes = new FileInfo(filePath).Length;
            UploadTaskStatus = UploadTaskStatus.Active;
        }

        public bool TaskOperate(UploadTaskStatus todo)
        {
            return false;
        }
    }
}
