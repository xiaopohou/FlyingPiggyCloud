using SixCloudCore.Controllers;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace SixCloudCore.ViewModels
{
    internal class DownloadingListViewModel : ViewModelBase
    {
        public ObservableCollection<DownloadingTaskViewModel> ObservableCollection => _observableCollection;
        private static readonly ObservableCollection<DownloadingTaskViewModel> _observableCollection = new ObservableCollection<DownloadingTaskViewModel>();

        /// <summary>
        /// 创建新的下载任务
        /// </summary>
        /// <param name="targetUUID">下载对象的uuid</param>
        /// <param name="downloadAddress">下载地址</param>
        /// <param name="localPath">本地路径</param>
        /// <param name="name">文件名</param>
        /// <param name="isAutoStart">自动开始</param>
        public static void NewTask(string targetUUID, string localPath, string name, bool isAutoStart = true)
        {
            DownloadingTaskViewModel task = new DownloadingTaskViewModel(targetUUID, localPath, name);
            //当下载任务结束时从列表中移除任务信息
            task.DownloadCompleted += (sender, e) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        _observableCollection.Remove(task);
                        if (File.Exists(task.CurrentFileFullPath))
                        {
                            DownloadedListViewModel.NewCompleted(task);
                        }
                    }
                    catch (Exception)
                    {

                    }
                });
            };
            Add(isAutoStart, task);
        }

        private static void Add(bool isAutoStart, DownloadingTaskViewModel task)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _observableCollection.Add(task);
            });
            if (isAutoStart)
            {
                task.Start();
            }
        }

        static DownloadingListViewModel()
        {
            TasksLogger.Downloadings = _observableCollection;
        }
    }

}
