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

    public enum UploadTaskStatus
    {
        Active,
        Pause,
        Abort,
        Completed,
        Error
    }
}
