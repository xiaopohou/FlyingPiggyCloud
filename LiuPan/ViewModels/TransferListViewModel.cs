using EzWcs;
using SixCloud.Controllers;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloud.ViewModels
{
    internal class TransferListViewModel
    {
        public DownloadingListViewModel DownloadingList { get; private set; } = new DownloadingListViewModel();

        public DownloadedListViewModel DownloadedList { get; private set; } = new DownloadedListViewModel();

        public UploadingListViewModel UploadingList { get; private set; } = new UploadingListViewModel();
    }

    internal class UploadingListViewModel : ViewModelBase
    {
        public ObservableCollection<UploadingTaskViewModel> ObservableCollection => _observableCollection;
        private static readonly ObservableCollection<UploadingTaskViewModel> _observableCollection = new ObservableCollection<UploadingTaskViewModel>();

        public static async Task NewTask(FileListViewModel targetList, string path)
        {
            if (Directory.Exists(path))
            {

            }
            else if (File.Exists(path))
            {
                UploadingFileViewModel task = await Task.Run(() => new UploadingFileViewModel(targetList.CurrentUUID, path));
                _observableCollection.Add(task);
                task.UploadCompleted += (sender, e) =>
                {
                    _observableCollection.Remove(task);
#warning 这里的代码还没有写完
                };
            }
            else
            {
                MessageBox.Show("由于找不到对象，6盘未能创建任务", "失败", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
    }

    internal class UploadingFileViewModel : UploadingTaskViewModel
    {
        public UploadingFileViewModel(string targetUUID, string filePath) : base()
        {
            Name = Path.GetFileName(filePath);
            Models.GenericResult<Models.UploadToken> x = fileSystem.UploadFile(Name, targetUUID, OriginalFilename: Name);
            task = EzWcs.EzWcs.NewTask(filePath, x.Result.Token, x.Result.UploadUrl);
        }

        private readonly IUploadTask task;

        public override string Uploaded => Calculators.SizeCalculator(task.CompletedBytes);

        public override string Total => Calculators.SizeCalculator(task.TotalBytes);

        public override double Progress => task.CompletedBytes * 100 / task.TotalBytes;

        public override UploadStatus Status
        {
            get
            {
                switch (task.UploadTaskStatus)
                {
                    case UploadTaskStatus.Active:
                        return UploadStatus.Running;
                    case UploadTaskStatus.Pause:
                        return UploadStatus.Pause;
                    case UploadTaskStatus.Abort:
                    case UploadTaskStatus.Error:
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
    }
}
