using System;
using System.Windows;
using System.Windows.Threading;

namespace SixCloudCore.ViewModels
{
    public interface ITransferItemViewModel
    {

        /// <summary>
        /// 用于定时刷新任务进度
        /// </summary>
        protected static readonly DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal, Application.Current.Dispatcher)
        {
            Interval = TimeSpan.FromSeconds(0.5d)
        };

        public string Icon { get; }

        public string Name { get; }

        public double Progress { get; }

        public TransferTaskStatus Status { get; }

        public string Completed { get; }

        public string Total { get; }

        public string Speed { get; }

        public DependencyCommand RecoveryCommand { get; }

        public DependencyCommand PauseCommand { get; }

        public DependencyCommand CancelCommand { get; }

        static ITransferItemViewModel()
        {
            timer.Start();
        }
    }
}
