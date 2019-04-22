namespace SixCloud.ViewModels
{
    internal class TransferListViewModel
    {
        public DownloadingListViewModel DownloadingList { get; private set; } = new DownloadingListViewModel();

        public DownloadedListViewModel DownloadedList { get; private set; } = new DownloadedListViewModel();

        public UploadingListViewModel UploadingList { get; private set; } = new UploadingListViewModel();

        public UploadedListViewModel UploadedList { get; private set; } = new UploadedListViewModel();
    }
}
