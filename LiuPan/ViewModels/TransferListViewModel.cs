namespace SixCloud.ViewModels
{
    internal class TransferListViewModel
    {
        public DownloadingListViewModel DownloadingList { get; private set; } = new DownloadingListViewModel();

        public DownloadedListViewModel DownloadedList { get; private set; } = new DownloadedListViewModel();
    }
}
