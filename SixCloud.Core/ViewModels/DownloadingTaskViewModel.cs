using QingzhenyunApis.Methods.V3;
using QingzhenyunApis.Utils;
using SixCloud.Core.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace SixCloud.Core.ViewModels
{
    public abstract class DownloadingTaskViewModel : ViewModelBase, ITransferItemViewModel
    {
        public string Icon { get; } = "\uf019";

        public abstract string Name { get; protected set; }

        public abstract string CurrentFileFullPath { get; }

        public abstract string Completed { get; }

        public abstract string TargetUUID { get; protected set; }

        public abstract string SavedLocalPath { get; protected set; }

        public abstract string Total { get; }

        public abstract double Progress { get; }

        public abstract TransferTaskStatus Status { get; }

        public abstract string Speed { get; }

        public DependencyCommand RecoveryCommand { get; }
        protected abstract void Recovery(object parameter);

        private bool CanRecovery(object parameter)
        {
            return Status == TransferTaskStatus.Pause;
        }


        public DependencyCommand PauseCommand { get; }
        protected abstract void Pause(object parameter);
        private bool CanPause(object parameter)
        {
            return Status == TransferTaskStatus.Running;
        }


        public DependencyCommand CancelCommand { get; }
        protected abstract void Cancel(object parameter);

        public abstract event EventHandler DownloadCompleted;

        public abstract event EventHandler DownloadCanceled;

        public DownloadingTaskViewModel()
        {
            RecoveryCommand = new DependencyCommand(Recovery, CanRecovery);
            PauseCommand = new DependencyCommand(Pause, CanPause);
            CancelCommand = new DependencyCommand(Cancel, DependencyCommand.AlwaysCan);
        }

    }

    public class StatusToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((TransferTaskStatus)value) == TransferTaskStatus.Running ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
