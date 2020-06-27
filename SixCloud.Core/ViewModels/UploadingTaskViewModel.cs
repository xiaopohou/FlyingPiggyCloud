using System;
using System.Windows;
using System.Windows.Threading;

namespace SixCloud.Core.ViewModels
{
    /// <summary>
    /// 上传任务的抽象类，用于派生单例上传任务和文件夹上传任务
    /// </summary>
    public abstract class UploadingTaskViewModel : ViewModelBase, ITransferItemViewModel
    {
        ///// <summary>
        ///// 用于定时刷新任务进度
        ///// </summary>
        //private static readonly DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal, Application.Current.Dispatcher)
        //{
        //    Interval = TimeSpan.FromSeconds(0.5d)
        //};

        static UploadingTaskViewModel()
        {

        }

        protected UploadingTaskViewModel()
        {
            RecoveryCommand = new DependencyCommand(Recovery, CanRecovery);
            PauseCommand = new DependencyCommand(Pause, CanPause);
            CancelCommand = new DependencyCommand(Stop, DependencyCommand.AlwaysCan);

            WeakEventManager<DispatcherTimer, EventArgs>.AddHandler(ITransferItemViewModel.timer, nameof(ITransferItemViewModel.timer.Tick), Callback);

            void Callback(object sender, EventArgs e)
            {
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(Completed));
                OnPropertyChanged(nameof(Progress));
                if (Status == TransferTaskStatus.Completed)
                {
                    UploadCompleted?.Invoke(this, new EventArgs());
                }
                else if (Status == TransferTaskStatus.Stop)
                {
                    UploadAborted?.Invoke(this, new EventArgs());
                }

                RecoveryCommand.OnCanExecutedChanged(this, null);
                PauseCommand.OnCanExecutedChanged(this, null);

            }
        }

        public string Icon { get; protected set; }

        public string Name { get; protected set; }

        public virtual TransferTaskStatus Status { get; }

        public abstract string Completed { get; }

        public abstract string Total { get; }

        public abstract double Progress { get; }

        public DependencyCommand PauseCommand { get; protected set; }
        protected abstract void Pause(object parameter);
        protected virtual bool CanPause(object parameter)
        {
            return Status == TransferTaskStatus.Running;
        }

        public DependencyCommand RecoveryCommand { get; protected set; }
        protected abstract void Recovery(object parameter);
        protected virtual bool CanRecovery(object parameter)
        {
            return Status == TransferTaskStatus.Pause;
        }

        public DependencyCommand CancelCommand { get; protected set; }
        public abstract void Stop(object parameter);

        public event EventHandler UploadCompleted;

        public virtual event EventHandler UploadAborted;

        public string LocalFilePath { get; protected set; }

        public string TargetPath { get; protected set; }
        public abstract string FriendlySpeed { get; }
    }
}
