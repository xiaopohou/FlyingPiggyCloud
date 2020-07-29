using QingzhenyunApis.Utils;
using SixCloud.Core.Models.Download;
using SixCloud.Core.Views.Dialogs;
using SixCloudCore.SixTransporter.Downloader;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SixCloud.Core.ViewModels
{
    public sealed class DownloadTaskViewModel : ViewModelBase, ITransferItemViewModel
    {
        private readonly ITaskManual innerTask;

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; } = "\uf019";

        /// <summary>
        /// 任务ID
        /// </summary>
        public Guid Guid => innerTask.Guid;

        /// <summary>
        /// 任务组ID
        /// </summary>
        public Guid Parent => innerTask.Parent;

        /// <summary>
        /// 本地文件名
        /// </summary>
        public string Name => innerTask.LocalFileName;

        /// <summary>
        /// 远程文件Identity
        /// </summary>
        public string TargetUUID => innerTask.TargetUUID;

        /// <summary>
        /// 本地保存路径
        /// </summary>
        public string LocalDirectory => innerTask.LocalDirectory;

        /// <summary>
        /// 任务状态
        /// </summary>
        public TransferTaskStatus Status { get; set; }

        /// <summary>
        /// 已完成
        /// </summary>
        public string Completed
        {
            get
            {
                if (innerTask is CommonFileDownloadTask common)
                {
                    return Calculators.SizeCalculator(common.Info.DownloadedSize);
                }
                else if (innerTask is DirectoryDownloadTask directory)
                {
                    if (directory.Initialized)
                    {
                        return $"已完成{directory.Completed}";
                    }
                    else
                    {
                        return $"正在枚举下载项";
                    }
                }
                else
                {
                    return Calculators.SizeCalculator(0);
                }
            }
        }

        /// <summary>
        /// 总计
        /// </summary>
        public string Total
        {
            get
            {
                if (innerTask is CommonFileDownloadTask common)
                {
                    return Calculators.SizeCalculator(common.Info.ContentSize);
                }
                else if (innerTask is DirectoryDownloadTask directory)
                {
                    if (directory.Initialized)
                    {
                        return $"共计{directory.Total}项";
                    }
                    else
                    {
                        return $"已发现{directory.Total}项";
                    }
                }
                else
                {
                    return Calculators.SizeCalculator(0);
                }
            }
        }

        /// <summary>
        /// 进度
        /// </summary>
        public double Progress
        {
            get
            {
                if (innerTask is CommonFileDownloadTask common)
                {
                    return common.Info.ContentSize == 0 ? 0 : common.Info.DownloadedSize * 100 / common.Info.ContentSize;
                }
                else if (innerTask is DirectoryDownloadTask directory)
                {
                    if (directory.Initialized)
                    {
                        if (directory.Total == 0)
                        {
                            Cancel(null);
                            return 0;
                        }
                        else
                        {
                            return directory.Completed * 100 / directory.Total;
                        }
                    }
                    else
                    {
                        return 0d;
                    }
                }
                else
                {
                    return 100d;
                }
            }

        }

        public string FriendlySpeed => innerTask is CommonFileDownloadTask common ? Calculators.SizeCalculator(common.Speed) + "/秒" : string.Empty;

        public event EventHandler TaskComplete;

        public event EventHandler TaskCancel;

        #region Commands
        public DependencyCommand RecoveryCommand { get; }
        private void Recovery(object parameter)
        {
            Status = TransferTaskStatus.Running;
            RecoveryCommand.OnCanExecutedChanged(this, EventArgs.Empty);
            if (!innerTask.IsCompleted)
            {
                if (innerTask is CommonFileDownloadTask common)
                {
                    common.Run();
                }
                else if (innerTask is DirectoryDownloadTask directory)
                {
                    directory.Run();
                }
            }
            PauseCommand.OnCanExecutedChanged(this, EventArgs.Empty);
        }

        private bool CanRecovery(object parameter)
        {
            return Status == TransferTaskStatus.Pause;
        }


        public DependencyCommand PauseCommand { get; }
        private void Pause(object parameter)
        {
            Status = TransferTaskStatus.Pause;
            PauseCommand.OnCanExecutedChanged(this, EventArgs.Empty);
            if (!innerTask.IsCompleted)
            {
                if (innerTask is DirectoryDownloadTask directory)
                {
                    directory.Running = false;
                }
                innerTask.Stop();
            }
            RecoveryCommand.OnCanExecutedChanged(this, EventArgs.Empty);
        }
        private bool CanPause(object parameter)
        {
            return Status == TransferTaskStatus.Running;
        }


        public DependencyCommand CancelCommand { get; }
        private void Cancel(object parameter)
        {
            Status = TransferTaskStatus.Stop;
            PauseCommand.OnCanExecutedChanged(this, EventArgs.Empty);
            RecoveryCommand.OnCanExecutedChanged(this, EventArgs.Empty);
            innerTask.Cancel();
            TaskCancel?.Invoke(this, EventArgs.Empty);
        }

        public DependencyCommand ShowDetailsCommand { get; }
        private async void ShowDetails(object parameter)
        {
            if (innerTask is DirectoryDownloadTask directoryDownloadTask && directoryDownloadTask.Initialized)
            {
                DirectoryDownloadTaskViewModel dataContext = await Task.Run(() => new DirectoryDownloadTaskViewModel(directoryDownloadTask));
                new DownloadTaskGroupDialog
                {
                    DataContext = dataContext,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                }.ShowDialog();
            }
        }
        private bool CanShowDetails(object parameter)
        {
            return innerTask is DirectoryDownloadTask;
        }
        #endregion

        public DownloadTaskViewModel(ITaskManual task)
        {
            innerTask = task;
            RecoveryCommand = new DependencyCommand(Recovery, CanRecovery);
            PauseCommand = new DependencyCommand(Pause, CanPause);
            CancelCommand = new DependencyCommand(Cancel, DependencyCommand.AlwaysCan);
            ShowDetailsCommand = new DependencyCommand(ShowDetails, CanShowDetails);

            //Status = TransferTaskStatus.Running;

            if (innerTask.IsCompleted)
            {
                Status = TransferTaskStatus.Completed;
            }

            if (innerTask is CommonFileDownloadTask common)
            {
                WeakEventManager<HttpDownloader, StatusChangedEventArgs>.AddHandler(common, nameof(common.DownloadStatusChangedEvent), (sender, e) => StatusConvert(e.NewValue));
                StatusConvert(common.Status);

                void StatusConvert(DownloadStatusEnum e)
                {
                    Status = e switch
                    {
                        DownloadStatusEnum.Downloading => TransferTaskStatus.Running,
                        DownloadStatusEnum.Waiting => TransferTaskStatus.Running,

                        DownloadStatusEnum.Paused => TransferTaskStatus.Pause,
                        DownloadStatusEnum.Failed => TransferTaskStatus.Failed,
                        DownloadStatusEnum.Completed => TransferTaskStatus.Completed,
                        _ => throw new InvalidCastException()
                    };
                }
            }

            WeakEventManager<ITaskManual, EventArgs>.AddHandler(innerTask, nameof(innerTask.TaskComplete), (sender, e) =>
            {
                Status = TransferTaskStatus.Completed;
                OnPropertyChanged(nameof(Status));
                PauseCommand.OnCanExecutedChanged(this, EventArgs.Empty);
                RecoveryCommand.OnCanExecutedChanged(this, EventArgs.Empty);

                //必须最后触发this.TaskComplete，否则对象会被GC
                Application.Current.Dispatcher.BeginInvoke(() => TaskComplete?.Invoke(sender, e));
            });

            WeakEventManager<DispatcherTimer, EventArgs>.AddHandler(ITransferItemViewModel.timer, nameof(ITransferItemViewModel.timer.Tick), TimerCallBack);

            void TimerCallBack(object sender, EventArgs e)
            {
                OnPropertyChanged(nameof(Completed));
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(FriendlySpeed));
            };
        }
    }
}
