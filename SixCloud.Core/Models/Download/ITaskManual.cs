using System;

namespace SixCloud.Core.Models.Download
{
    /// <summary>
    /// 下载任务信息
    /// </summary>
    public interface ITaskManual
    {
        /// <summary>
        /// 远程Identity
        /// </summary>
        string TargetUUID { get; }

        /// <summary>
        /// 任务ID
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// 任务组ID，单任务时为Empty
        /// </summary>
        Guid Parent { get; }

        /// <summary>
        /// 本地文件名
        /// </summary>
        string LocalFileName { get; }

        /// <summary>
        /// 本地保存路径
        /// </summary>
        string LocalDirectory { get; }

        bool IsCompleted { get; }

        void Run();

        void Stop();

        void Cancel();

        event EventHandler TaskCompleted;
    }
}