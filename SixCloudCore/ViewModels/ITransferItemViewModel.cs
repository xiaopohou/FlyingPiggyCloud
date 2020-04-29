namespace SixCloudCore.ViewModels
{
    public interface ITransferItemViewModel
    {
        public string Icon { get; set; }

        public string Name { get; set; }

        public double Progress { get; }

        public string Status { get; set; }

        public string Completed { get; set; }

        public string Total { get; set; }

        public string Speed { get; set; }

        public DependencyCommand RecoveryCommand { get; set; }

        public DependencyCommand PauseCommand { get; set; }

        public DependencyCommand CancelCommand { get; set; }
    }
}
