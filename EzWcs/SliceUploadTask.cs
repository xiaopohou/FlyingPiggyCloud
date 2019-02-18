using System;
using System.IO;

namespace EzWcs
{
    internal class SliceUploadTask : IUploadTask
    {
        public string FilePath { get; protected set; }

        public string Token { get; protected set; }

        public string Address { get; protected set; }

        public long CompletedBlockCount { get; set; } = 0;

        public long TotalBlockCount { get; set; }

        public string[] TotalContents { get; set; }

        public string UploadUrl { get; protected set; }

        public string UploadBatch { get; protected set; }

        public UploadTaskStatus UploadTaskStatus { get; set; }

        public SliceUploadTask(string filePath, string token, string uploadUrl)
        {
            FilePath = filePath;
            Token = token;
            UploadUrl = uploadUrl;
            UploadBatch = Guid.NewGuid().ToString();
            TotalBlockCount = (new FileInfo(filePath).Length + SliceUploadWorker.BLOCKSIZE - 1) / SliceUploadWorker.BLOCKSIZE;
        }
    }
}
