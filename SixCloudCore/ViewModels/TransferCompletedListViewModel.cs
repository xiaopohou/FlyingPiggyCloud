﻿using System;
using System.Collections.ObjectModel;

namespace SixCloudCore.ViewModels
{
    internal class TransferCompletedListViewModel : ViewModelBase
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

        public static void NewDownloadedTask(DownloadingTaskViewModel task)
        {
            DownloadedTaskViewModel record = new DownloadedTaskViewModel(task);
            App.Current.Dispatcher.Invoke(() => transferCompletedList.Insert(0, record));
            record.Deleted += RemoveAfterDeleted;

            void RemoveAfterDeleted(object sender, EventArgs e)
            {
                record.Deleted -= RemoveAfterDeleted;
                App.Current.Dispatcher.Invoke(() => transferCompletedList.Remove(record));
            }
        }


    }

}