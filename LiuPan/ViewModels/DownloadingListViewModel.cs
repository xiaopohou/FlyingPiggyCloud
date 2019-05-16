using SixCloud.Controllers;
using SixCloud.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using static SixCloud.Models.OfflineTaskList;

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
    }

    internal class OfflineTaskViewModel : ViewModelBase
    {
        public ObservableCollection<OfflineTask> ObservableCollection { get; } = new ObservableCollection<OfflineTask>();
        private readonly OfflineDownloader downloader = new OfflineDownloader();

        private IEnumerator<OfflineTask[]> listEnumerator;
        private IEnumerable<OfflineTask[]> GetListEnumerator()
        {
            int currentPage = 0;
            int totalPage = 0;
            do
            {
                GenericResult<OfflineTaskList> x = downloader.GetList(++currentPage);
                totalPage = x.Result.TotalPage;
                yield return x.Result.List;
            } while (currentPage < totalPage);
            yield break;
        }

        public void LazyLoad()
        {
            if (listEnumerator == null)
            {
                listEnumerator = GetListEnumerator().GetEnumerator();
            }
            if (listEnumerator.MoveNext())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (OfflineTask a in listEnumerator.Current)
                    {
                        ObservableCollection.Add(a);
                    }
                });
            }
        }


        #region Commands
        public DependencyCommand NewTaskCommand { get; set; }
        private void NewTask(object parameters)
        {
#warning 离线下载新建任务的逻辑没有写完，缺乏多个视图配合
        }

        public DependencyCommand CancelTaskCommand { get; set; }
        private void CancelTask(object parameters)
        {
            
        }
        #endregion



        public OfflineTaskViewModel()
        {
            NewTaskCommand = new DependencyCommand(NewTask, DependencyCommand.AlwaysCan);
            CancelTaskCommand = new DependencyCommand(CancelTask, DependencyCommand.AlwaysCan);
        }
    }

}
