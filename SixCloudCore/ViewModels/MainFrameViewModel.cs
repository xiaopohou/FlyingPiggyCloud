using QingzhenyunApis.EntityModels;

namespace SixCloudCore.ViewModels
{
    internal sealed class MainFrameViewModel : ViewModelBase
    {
        public RecoveryBoxViewModel RecVM { get; set; } = new RecoveryBoxViewModel();

        public FileListViewModel FileVM { get; set; } = new FileListViewModel();

        public DownloadingListViewModel DownloadingList { get; private set; } = new DownloadingListViewModel();

        public DownloadedListViewModel DownloadedList { get; private set; } = new DownloadedListViewModel();

        public UploadingListViewModel UploadingList { get; private set; } = new UploadingListViewModel();

        public UploadedListViewModel UploadedList { get; private set; } = new UploadedListViewModel();

#warning 此处暂未实现
        //public OfflineTaskViewModel OfflineTask { get; private set; } = new OfflineTaskViewModel();


        public MainFrameViewModel()
        {
        }
    }
}
