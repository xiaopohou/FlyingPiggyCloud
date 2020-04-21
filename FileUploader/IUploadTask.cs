using SixTransporter.UploadEngine;
using System;

namespace SixCloudCore.FileUploader
{
    public interface IUploadTask
    {
        /// <summary>
        /// 请求上传的文件路径
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// 请求的Token
        /// </summary>
        string Token { get; }

        /// <summary>
        /// 地址
        /// </summary>
        string Address { get; }

        long CompletedBytes { get; }

        string Hash { get; }

        long TotalBytes { get; }

        UploadTaskStatus UploadTaskStatus { get; }

        /// <summary>
        /// 尝试将当前任务变更为指定状态
        /// </summary>
        /// <param name="todo">可选的取值：Active/Pause/Abort</param>
        /// <returns>指示状态变更是否成功</returns>
        bool TaskOperate(UploadTaskStatus todo);
    }

    public class UploadTask : IUploadTask
    {
        private SixUploader SixUploader { get; }

        public string FilePath => SixUploader.Info.FilePath;

        public string Token => SixUploader.Info.Token;

        public string Address => SixUploader.Info.UploadUrl;

        public long CompletedBytes => SixUploader.UploadedSize;

        public string Hash => SixUploader.Info.FileHash;

        public long TotalBytes => SixUploader.Info.FileSize;

        public UploadTaskStatus UploadTaskStatus
        {
            get
            {
                return SixUploader.Status switch
                {
                    UploadTaskStatusEnum.Completed => UploadTaskStatus.Completed,
                    UploadTaskStatusEnum.Faulted => UploadTaskStatus.Error,
                    UploadTaskStatusEnum.Hashing => UploadTaskStatus.Active,
                    UploadTaskStatusEnum.Paused => UploadTaskStatus.Pause,
                    UploadTaskStatusEnum.Uploading => UploadTaskStatus.Active,
                    UploadTaskStatusEnum.Waiting => UploadTaskStatus.Active,
                    _ => throw new ArgumentException(),
                };
            }
        }

        internal UploadTask(SixUploader sixUploader)
        {
            SixUploader = sixUploader;
        }

        public bool TaskOperate(UploadTaskStatus todo)
        {
            throw new System.NotImplementedException();
        }
    }

    public enum UploadTaskStatus
    {
        Active,
        Pause,
        Abort,
        Completed,
        Error
    }
}
