namespace SixCloudCore.ViewModels
{
    public interface ITransferItemViewModel
    {
        public string Icon { get; }

        public string Name { get; }

        public double Progress { get; }

        public TransferTaskStatus Status { get;}

        public string Completed { get; }

        public string Total { get; }

        public string Speed { get;}

        public DependencyCommand RecoveryCommand { get; }

        public DependencyCommand PauseCommand { get;}

        public DependencyCommand CancelCommand { get; }
    }

    public enum TransferTaskStatus
    {
        Running,
        Pause,
        Stop,
        Completed,
    }
}
