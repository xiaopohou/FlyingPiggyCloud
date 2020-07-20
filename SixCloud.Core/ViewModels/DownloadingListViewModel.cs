using SixCloud.Core.Models.Download;
using System.Collections.ObjectModel;

namespace SixCloud.Core.ViewModels
{
    internal class DownloadingListViewModel : TransferListViewModel
    {
        public ObservableCollection<DownloadTaskViewModel> ObservableCollection => DownloadingList;

        public static async void NewTask(string targetUUID, string localPath, string name, bool isAutoStart = true)
        {
            await NewDownloadTask(targetUUID, localPath, name, isAutoStart);
        }
    }

}
