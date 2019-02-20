using System;

namespace SixCloud.Models
{
    public class SingleFileUploadTask : IUploadTask
    {
        /// <summary>
        /// 待上传文件的绝对路径
        /// </summary>
        private readonly string fullPath;

        private readonly string parentPath;

        private readonly string parentUUID;

        public SingleFileUploadTask(string fullPath, string parentPath = null, string parentUUID = null)
        {
            this.fullPath = fullPath ?? throw new ArgumentNullException(nameof(fullPath));
            this.parentPath = parentPath;
            this.parentUUID = parentUUID;
        }

        /// <summary>
        /// 待上传文件的文件名
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 已上传的字节数
        /// </summary>
        public long UploadedBytes { get; private set; }

        /// <summary>
        /// 文件的总字节数
        /// </summary>
        public long TotalBytes { get; private set; }

        /// <summary>
        /// 上传进度
        /// </summary>
        public double Progress
        {
            get
            {
                if (TotalBytes != 0)
                {
                    return UploadedBytes * 100 / TotalBytes;
                }
                else
                {
                    return 0D;
                }
            }
        }

        public string Status { get; set; }

        public bool IsRunning { get; private set; } = false;

        public void Abort()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public RecoverableUploadTaskArchive Save()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}
