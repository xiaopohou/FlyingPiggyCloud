using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods;
using SixCloud.Controllers;
using SixCloud.Models;
using SixCloud.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloud.ViewModels
{
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
            Application.Current.Dispatcher.Invoke(() =>
            {
                OfflineUrlsDialog a = new OfflineUrlsDialog();
                a.Show();
                a.DataContext = new OfflineUrlsDialogViewModel(a);
            });
        }

        public DependencyCommand CancelTaskCommand { get; set; }
        private async void CancelTask(object parameters)
        {
            IList list = parameters as IList;
            if (list.Count > 0)
            {
                IEnumerable<OfflineTask> cancellingTasks = list.Cast<OfflineTask>();
                List<string> taskID = new List<string>(list.Count);
                foreach (OfflineTask task in cancellingTasks)
                {
                    taskID.Add(task.Identity);
                }
                await Task.Run(() => downloader.DeleteTask(taskID.ToArray()));
                RefreshList();
            }
        }


        public DependencyCommand RefreshListCommand { get; set; }
        private void RefreshList(object parameter=null)
        {
            ObservableCollection.Clear();
            listEnumerator = null;
            LazyLoad();
        }
        #endregion



        public OfflineTaskViewModel()
        {
            NewTaskCommand = new DependencyCommand(NewTask, DependencyCommand.AlwaysCan);
            CancelTaskCommand = new DependencyCommand(CancelTask, DependencyCommand.AlwaysCan);
            RefreshListCommand = new DependencyCommand(RefreshList, DependencyCommand.AlwaysCan);
            LazyLoad();
        }
    }

}
