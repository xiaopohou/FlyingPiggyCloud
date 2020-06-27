using Newtonsoft.Json;
using QingzhenyunApis.Utils;
using SixCloud.Core.ViewModels;
using System;
using System.IO;

namespace SixCloud.Core.Models
{
    public class EmptyFileDownloadTask : DownloadingTaskViewModel, IDownloadTask
    {
        private TransferTaskStatus status = TransferTaskStatus.Pause;

        public override string Completed => Calculators.SizeCalculator(0);

        public long CompletedBytes { get; } = 0;

        /// <summary>
        /// 下载任务在本地保存的绝对路径
        /// </summary>
        public override string CurrentFileFullPath { get; }

        public override string Name { get; protected set; }

        public override double Progress { get; } = 100d;

        public override string SavedLocalPath { get; protected set; }

        public override string FriendlySpeed => Calculators.SizeCalculator(0) + "/秒";

        public override TransferTaskStatus Status { get => status; }
        public override string TargetUUID { get; protected set; }

        public override string Total => Calculators.SizeCalculator(0);

        protected override void Recovery(object parameter)
        {
            status = TransferTaskStatus.Running;
            OnPropertyChanged(nameof(Status));
            if (!Directory.Exists(SavedLocalPath))
            {
                Directory.CreateDirectory(SavedLocalPath);
            }

            File.Create(CurrentFileFullPath).Close();
            status = TransferTaskStatus.Completed;
            OnPropertyChanged(nameof(Status));
            DownloadCompleted?.Invoke(this, null);
        }

        public override event EventHandler DownloadCanceled;
        public override event EventHandler DownloadCompleted;

        public EmptyFileDownloadTask(string storagePath, string name, string targetUUID)
        {
            Name = name;
            TargetUUID = targetUUID;
            SavedLocalPath = storagePath;
            CurrentFileFullPath = Path.Combine(SavedLocalPath, Name);

        }

        public DownloadTaskRecord ToRecord()
        {
            return new DownloadTaskRecord
            {
                LocalPath = SavedLocalPath,
                TargetUUID = TargetUUID,
                Name = Name,
            };
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(ToRecord());
        }

        protected override void Pause(object parameter)
        {
            throw new NotImplementedException();
        }

        protected override void Cancel(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}