using SixCloudCore.Controllers;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace SixCloudCore.ViewModels
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
