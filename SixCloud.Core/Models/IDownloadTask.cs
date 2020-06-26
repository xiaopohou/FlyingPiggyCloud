using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods.V3;
using SixCloud.Core.ViewModels;
using System;

namespace SixCloud.Core.Models
{
    public interface IDownloadTask
    {
        string Completed { get; }
        long CompletedBytes { get; }
        string CurrentFileFullPath { get; }

        /// <summary>
        /// 下载任务的保存文件名
        /// </summary>
        string Name { get; }
        double Progress { get; }

        /// <summary>
        /// 下载任务的保存目录
        /// </summary>
        string SavedLocalPath { get; }
        string Speed { get; }
        TransferTaskStatus Status { get; }
        string TargetUUID { get; }
        string Total { get; }

        event EventHandler DownloadCanceled;
        event EventHandler DownloadCompleted;

        DependencyCommand RecoveryCommand { get; }

        string ToString();

        DownloadTaskRecord ToRecord();
    }
}