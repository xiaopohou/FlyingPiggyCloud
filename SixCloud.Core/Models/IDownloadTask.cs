using QingzhenyunApis.Utils;
using SixCloud.Core.ViewModels;
using System;

namespace SixCloud.Core.Models
{
    public interface IDownloadTask
    {
        string Completed { get; }
        long CompletedBytes { get; }
        string CurrentFileFullPath { get; }
        string Name { get; }
        double Progress { get; }
        string SavedLocalPath { get; }
        string Speed { get; }
        TransferTaskStatus Status { get; }
        string TargetUUID { get; }
        string Total { get; }

        event EventHandler DownloadCanceled;
        event EventHandler DownloadCompleted;

        DependencyCommand RecoveryCommand { get; }


        string ToString();
    }

    public class EmptyFileDownloadTask : IDownloadTask
    {
        public string Completed => throw new NotImplementedException();

        public long CompletedBytes => throw new NotImplementedException();

        public string CurrentFileFullPath => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public double Progress => throw new NotImplementedException();

        public string SavedLocalPath => throw new NotImplementedException();

        public string Speed => Calculators.SizeCalculator(0) + "/秒";

        public TransferTaskStatus Status { get; set; }

        public string TargetUUID => throw new NotImplementedException();

        public string Total => throw new NotImplementedException();

        public DependencyCommand RecoveryCommand { get; }
        private void Recovery(object parameter)
        {

        }

        public event EventHandler DownloadCanceled;
        public event EventHandler DownloadCompleted;

        public EmptyFileDownloadTask()
        {
            RecoveryCommand = new DependencyCommand(Recovery);
        }
    }
}