using QingzhenyunApis.EntityModels;

namespace SixCloudCore.ViewModels
{
    internal sealed class MainFrameViewModel : ViewModelBase
    {
        public RecoveryBoxViewModel RecVM { get; set; } = new RecoveryBoxViewModel();

        public FileListViewModel FileVM { get; set; } = new FileListViewModel();

        public DownloadedListViewModel DownloadedList { get; private set; } = new DownloadedListViewModel();

        public TransferListViewModel TransferList { get; private set; } = new TransferListViewModel();

        public UploadedListViewModel UploadedList { get; private set; } = new UploadedListViewModel();

        public OfflineTaskViewModel OfflineTask { get; private set; } = new OfflineTaskViewModel();

        public UserInformationViewModel UserInformation { get; private set; } = new UserInformationViewModel();
    }
}
