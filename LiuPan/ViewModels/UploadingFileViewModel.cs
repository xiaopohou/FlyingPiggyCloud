using EzWcs;
using SixCloud.Controllers;
using System;
using System.IO;

namespace SixCloud.ViewModels
{
    internal class UploadingFileViewModel : UploadingTaskViewModel
    {
        public UploadingFileViewModel(string targetUUID, string filePath) : base()
        {
            Name = Path.GetFileName(filePath);
            Models.GenericResult<Models.UploadToken> x = fileSystem.UploadFile(Name, targetUUID, OriginalFilename: Name);
            task = EzWcs.EzWcs.NewTask(filePath, x.Result.UploadInfo.Token, x.Result.UploadInfo.UploadUrl);
        }

        private readonly IUploadTask task;

        public override string Uploaded => Calculators.SizeCalculator(task.CompletedBytes);

        public override string Total => Calculators.SizeCalculator(task.TotalBytes);

        public override double Progress => task.CompletedBytes * 100 / task.TotalBytes;

        public override UploadStatus Status
        {
            get
            {
                if (task == null)
                {
                    return UploadStatus.Pause;
                }
                switch (task.UploadTaskStatus)
                {
                    case UploadTaskStatus.Active:
                        return UploadStatus.Running;
                    case UploadTaskStatus.Pause:
                    case UploadTaskStatus.Error:
                        return UploadStatus.Pause;
                    case UploadTaskStatus.Abort:
                        return UploadStatus.Stop;
                    case UploadTaskStatus.Completed:
                        return UploadStatus.Completed;
                }
                return UploadStatus.Running;
            }
        }

        public override void Stop(object parameter)
        {
            task.TaskOperate(UploadTaskStatus.Abort);
        }

        protected override void Pause()
        {
            task.TaskOperate(UploadTaskStatus.Pause);
        }

        protected override void Start()
        {
            task.TaskOperate(UploadTaskStatus.Active);
        }

        //public override event EventHandler UploadAborted;

#if DEBUG
        ~UploadingFileViewModel()
        {
            Console.WriteLine("已回收");
        }
#endif

    }
}
