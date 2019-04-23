using System;
using System.Collections.ObjectModel;

namespace SixCloud.ViewModels
{
    internal class DownloadedListViewModel : ViewModelBase
    {
        public ObservableCollection<DownloadedTaskViewModel> ObservableCollection => _observableCollection;
        private static readonly ObservableCollection<DownloadedTaskViewModel> _observableCollection = new ObservableCollection<DownloadedTaskViewModel>();


        public static void NewCompleted(DownloadingTaskViewModel task)
        {
            DownloadedTaskViewModel record = new DownloadedTaskViewModel(task);
            App.Current.Dispatcher.Invoke(() => _observableCollection.Insert(0, record));
            record.Deleted += RemoveAfterDeleted;
            void RemoveAfterDeleted(object sender, EventArgs e)
            {
                record.Deleted -= RemoveAfterDeleted;
                App.Current.Dispatcher.Invoke(() =>
                {
                    _observableCollection.Remove(record);
                });
            }
        }

    }

}
