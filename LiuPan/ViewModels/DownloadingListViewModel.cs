﻿using SixCloud.Controllers;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace SixCloud.ViewModels
{
    internal class DownloadingListViewModel : ViewModelBase
    {
        public ObservableCollection<DownloadingTaskViewModel> ObservableCollection => _observableCollection;
        private static readonly ObservableCollection<DownloadingTaskViewModel> _observableCollection;

        public static void NewTask(string targetUUID, string downloadAddress, string localPath, string name, bool isAutoStart = true)
        {
            DownloadingTaskViewModel task = new DownloadingTaskViewModel(targetUUID, downloadAddress, localPath, name);
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
            _observableCollection = new ObservableCollection<DownloadingTaskViewModel>();
            TasksLogger.Downloadings = _observableCollection;
        }
    }

}
