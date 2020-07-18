using System;

namespace SixCloud.Core.ViewModels
{
    public abstract class DownloadTaskViewModel : ViewModelBase, ITransferItemViewModel
    {
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; } = "\uf019";

        /// <summary>
        /// 任务ID
        /// </summary>
        public Guid Guid { get; protected set; }

        /// <summary>
        /// 任务组ID
        /// </summary>
        public Guid Parent { get; protected set; }

        /// <summary>
        /// 本地文件名
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 远程文件Identity
        /// </summary>
        public string TargetUUID { get; protected set; }

        /// <summary>
        /// 本地保存路径
        /// </summary>
        public string LocalDirectory { get; protected set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public virtual TransferTaskStatus Status { get; protected set; }

        /// <summary>
        /// 已完成
        /// </summary>
        public abstract string Completed { get; }

        /// <summary>
        /// 总计
        /// </summary>
        public abstract string Total { get; }

        /// <summary>
        /// 进度
        /// </summary>
        public abstract double Progress { get; }

        public abstract string FriendlySpeed { get; }

        #region Commands
        public DependencyCommand RecoveryCommand { get; }
        protected abstract void Recovery(object parameter);

        protected virtual bool CanRecovery(object parameter)
        {
            return Status == TransferTaskStatus.Pause;
        }


        public DependencyCommand PauseCommand { get; }
        protected abstract void Pause(object parameter);
        protected virtual bool CanPause(object parameter)
        {
            return Status == TransferTaskStatus.Running;
        }


        public DependencyCommand CancelCommand { get; }
        protected bool Cancelled { get; set; } = false;
        protected abstract void Cancel(object parameter);


        #endregion

        public virtual event EventHandler DownloadCompleted;

        public virtual event EventHandler DownloadCanceled;

        protected DownloadTaskViewModel()
        {
            RecoveryCommand = new DependencyCommand(Recovery, CanRecovery);
            PauseCommand = new DependencyCommand(Pause, CanPause);
            CancelCommand = new DependencyCommand(Cancel, DependencyCommand.AlwaysCan);

            DownloadCanceled += (sender, e) =>
            {
                lock (ITransferItemViewModel.taskList)
                {
                    ITransferItemViewModel.taskList.Remove(this);
                }
            };
        }
    }
}
