using SixCloudCore.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloudCore.ViewModels
{
    internal class UploadingListViewModel : TransferListViewModel
    {
        public ObservableCollection<UploadingTaskViewModel> ObservableCollection => uploadingList;

        public static async Task NewTask(FileListViewModel targetList, string path)
        {
            await NewUploadTask(targetList.CurrentPath, path);
        }

        public static async Task NewTask(string targetPath, string path)
        {
            await NewUploadTask(targetPath, path);
        }

    }
}
