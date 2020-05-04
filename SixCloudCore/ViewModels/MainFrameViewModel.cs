namespace SixCloudCore.ViewModels
{
    internal sealed class MainFrameViewModel : ViewModelBase
    {
        public RecoveryBoxViewModel RecVM { get; set; } = new RecoveryBoxViewModel();

        public FileListViewModel FileVM { get; set; } = new FileListViewModel();

        public TransferListViewModel TransferList { get; private set; } = new TransferListViewModel();

        public OfflineTaskViewModel OfflineTask { get; private set; } = new OfflineTaskViewModel();

        public UserInformationViewModel UserInformation { get; private set; } = new UserInformationViewModel();
    }
}
