namespace EzWcs
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

        long TotalBytes { get; }

        UploadTaskStatus UploadTaskStatus { get; set; }
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
