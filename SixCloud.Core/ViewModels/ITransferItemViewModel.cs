using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SixCloud.Core.ViewModels
{
    public interface ITransferItemViewModel
    {
        public sealed class Refresher
        {
            public event EventHandler Tick;

            internal Refresher()
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        Tick?.Invoke(this, EventArgs.Empty);
                        Thread.Sleep(500);
                    }
                });
            }
        }

        /// <summary>
        /// 用于定时刷新任务进度
        /// </summary>
        protected static readonly Refresher refresher = new Refresher();

        public string Icon { get; }

        public string Name { get; }

        public double Progress { get; }

        public TransferTaskStatus Status { get; }

        public string Completed { get; }

        public string Total { get; }

        public string FriendlySpeed { get; }

        public DependencyCommand RecoveryCommand { get; }

        public DependencyCommand PauseCommand { get; }

        public DependencyCommand CancelCommand { get; }
    }
}
