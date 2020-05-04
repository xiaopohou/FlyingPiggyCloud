using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods.V3;
using QingzhenyunApis.Utils;
using SixCloudCore.FileUploader;
using SixCloudCore.FileUploader.Calculators;
using System;
using System.IO;

namespace SixCloudCore.ViewModels
{
    internal partial class UploadingFileViewModel : UploadingTaskViewModel
    {
        public UploadingFileViewModel(string targetPath, string filePath) : base()
        {
            TargetPath = targetPath;
            LocalFilePath = filePath;
            Name = Path.GetFileName(filePath);
            string hash = $"{ETag.ComputeEtag(filePath)}{Calculators.LongTo36(new FileInfo(filePath).Length)}";
            UploadToken x = FileSystem.UploadFile(Name, parentPath: targetPath, hash: hash, originalFilename: Name).Result;
            //if (x.HashCached)
            //{
            //    task = new HashCachedTask();
            //    return;
            //}
            task = EzWcs.NewTask(filePath, x.UploadTokenUploadToken, x.DirectUploadUrl, x.PartUploadUrl);
        }

        private readonly IUploadTask task;

        private DateTime lastTime;
        private long lastCompletedBytes;

        public override string Speed
        {
            get
            {
                if (lastTime == default || lastCompletedBytes == default)
                {
                    lastTime = DateTime.Now;
                    lastCompletedBytes = task.CompletedBytes;
                    return "0B/秒";
                }
                else
                {
                    var span = DateTime.Now - lastTime;
                    lastTime += span;
                    var intervalCompleted = task.CompletedBytes - lastCompletedBytes;
                    lastCompletedBytes += intervalCompleted;
                    return Calculators.SizeCalculator((long)Math.Round(span.TotalSeconds == 0 ? 0 : intervalCompleted / span.TotalSeconds, 0)) + "/秒";
                }
            }
        }

        public override string Completed => Calculators.SizeCalculator(task.CompletedBytes);

        public override string Total => Calculators.SizeCalculator(task.TotalBytes);

        public override double Progress => task.CompletedBytes * 100 / task.TotalBytes;

        public override TransferTaskStatus Status
        {
            get
            {
                if (task == null)
                {
                    return TransferTaskStatus.Pause;
                }
                switch (task.UploadTaskStatus)
                {
                    case UploadTaskStatus.Active:
                        return TransferTaskStatus.Running;
                    case UploadTaskStatus.Pause:
                    case UploadTaskStatus.Error:
                        return TransferTaskStatus.Pause;
                    case UploadTaskStatus.Abort:
                        return TransferTaskStatus.Stop;
                    case UploadTaskStatus.Completed:
                        return TransferTaskStatus.Completed;
                }
                return TransferTaskStatus.Running;
            }
        }

        public override void Stop(object parameter)
        {
            task.TaskOperate(UploadTaskStatus.Abort);

        }

        protected override void Pause(object parameter)
        {
            task.TaskOperate(UploadTaskStatus.Pause);
        }

        protected override void Recovery(object parameter)
        {
            task.TaskOperate(UploadTaskStatus.Active);
        }

#if DEBUG
        ~UploadingFileViewModel()
        {
            Console.WriteLine("已回收");
        }
#endif

    }

}
