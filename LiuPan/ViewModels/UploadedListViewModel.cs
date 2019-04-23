using System;
using System.Collections.ObjectModel;

namespace SixCloud.ViewModels
{
    internal class UploadedListViewModel : ViewModelBase
    {
        public ObservableCollection<UploadedTaskViewModel> ObservableCollection => _observableCollection;
        private static readonly ObservableCollection<UploadedTaskViewModel> _observableCollection = new ObservableCollection<UploadedTaskViewModel>();

        public static void NewTask(UploadingTaskViewModel uploadedTask)
        {
            UploadedTaskViewModel task = new UploadedTaskViewModel
            {
                Name = uploadedTask.Name,
                CompletedTime = DateTime.Now
            };
            _observableCollection.Insert(0, task);
        }
    }
}
