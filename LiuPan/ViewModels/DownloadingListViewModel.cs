using System;
using System.Collections.ObjectModel;

namespace SixCloud.ViewModels
{
    internal class DownloadingListViewModel : ViewModelBase
    {
        public ObservableCollection<DownloadingTaskViewModel> ObservableCollection => _observableCollection;
        private static readonly ObservableCollection<DownloadingTaskViewModel> _observableCollection = new ObservableCollection<DownloadingTaskViewModel>();

        public static void NewTask(string downloadAddress, string localPath, string name, bool isAutoStart = true)
        {
            DownloadingTaskViewModel task = new DownloadingTaskViewModel(downloadAddress, localPath, name);
            //当下载任务结束时从列表中移除任务信息
            task.DownloadCompleted += (sender, e) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        _observableCollection.Remove(task);
                    }
                    catch (Exception)
                    {

                    }
                });
            };
            App.Current.Dispatcher.Invoke(() =>
            {
                _observableCollection.Add(task);
            });
            if (isAutoStart)
            {
                task.Start();
            }
        }

    }
}
