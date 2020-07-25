using SixCloud.Core.Models.Download;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace SixCloud.Core.ViewModels
{
    public class TransferCompletedListViewModel : ViewModelBase
    {
        private static readonly ObservableCollection<ITransferCompletedTaskViewModel> transferCompletedList = new ObservableCollection<ITransferCompletedTaskViewModel>();

        public ObservableCollection<ITransferCompletedTaskViewModel> TransferCompletedList => transferCompletedList;

        public static void NewUploadedTask(UploadingTaskViewModel uploadedTask)
        {
            UploadedTaskViewModel task = new UploadedTaskViewModel
            {
                Name = uploadedTask.Name,
                CompletedTime = DateTime.Now
            };
            transferCompletedList.Insert(0, task);
        }

        public static void NewDownloadedTask(ITaskManual task)
        {
            DownloadedTaskViewModel record = new DownloadedTaskViewModel(task);
            Application.Current.Dispatcher.Invoke(() => transferCompletedList.Insert(0, record));
            record.Deleted += RemoveAfterDeleted;

            void RemoveAfterDeleted(object sender, EventArgs e)
            {
                record.Deleted -= RemoveAfterDeleted;
                Application.Current.Dispatcher.Invoke(() => transferCompletedList.Remove(record));
            }
        }

    }

}
