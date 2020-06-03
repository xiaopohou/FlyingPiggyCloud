using System.Collections.ObjectModel;

namespace SixCloud.Core.ViewModels
{
    internal class DownloadingListViewModel : TransferListViewModel
    {
        public ObservableCollection<DownloadingTaskViewModel> ObservableCollection => downloadingList;

        public static void NewTask(string targetUUID, string localPath, string name, bool isAutoStart = true)
        {
            NewDownloadTask(targetUUID, localPath, name, isAutoStart);
        }
    }

}
