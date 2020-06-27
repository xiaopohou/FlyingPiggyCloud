using QingzhenyunApis.Utils;
using SixCloud.Core.ViewModels;
using System;
using System.IO;

namespace SixCloud.Core.Models
{
    public class CompletedDownloadTask : IDownloadTask
    {
        public string Completed => Calculators.SizeCalculator(CompletedBytes);

        public long CompletedBytes { get; }

        public string CurrentFileFullPath { get; }

        public string Name { get; }

        public double Progress => 100d;

        public string SavedLocalPath { get; }

        public long Speed => 0;

        public string FriendlySpeed => Calculators.SizeCalculator(Speed) + "/秒";

        public TransferTaskStatus Status => TransferTaskStatus.Completed;

        public string TargetUUID { get; }

        public string Total => Completed;

        public DependencyCommand RecoveryCommand => throw new NotImplementedException();

        public event EventHandler DownloadCanceled;
        public event EventHandler DownloadCompleted;

        public DownloadTaskRecord ToRecord()
        {
            return new DownloadTaskRecord
            {
                LocalPath = SavedLocalPath,
                TargetUUID = TargetUUID,
                Name = Name,
            };
        }

        public CompletedDownloadTask(string storagePath, string name, string targetUUID)
        {
            Name = name;
            TargetUUID = targetUUID;
            SavedLocalPath = storagePath;
            CurrentFileFullPath = Path.Combine(SavedLocalPath, Name);
            CompletedBytes = new FileInfo(CurrentFileFullPath).Length;
        }
    }
}