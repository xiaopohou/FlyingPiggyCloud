using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using SixCloud.Core.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloud.Core.ViewModels
{
    public class OfflineTaskViewModel : ViewModelBase
    {
        public ObservableCollection<OfflineTask> ObservableCollection { get; private set; } = new ObservableCollection<OfflineTask>();

        private IAsyncEnumerator<OfflineTask> taskEnumerator;
        private async IAsyncEnumerable<OfflineTask> GetTaskEnumeratorAsync()
        {
            var skip = 0;
            const int limit = 20;
            int count;
            do
            {
                var x = await OfflineDownloader.GetList(skip, limit);
                count = x.DataList.Count;
                foreach (var item in x.DataList)
                {
                    yield return item;
                }
                skip += limit;
            } while (count == limit);
            yield break;
        }

        public async Task LazyLoad()
        {
            try
            {
                for (var count = 0; count < 20; count++)
                {
                    if (await taskEnumerator.MoveNextAsync())
                    {
                        Application.Current.Dispatcher.Invoke(() => ObservableCollection.Add(taskEnumerator.Current));
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (RequestFailedException ex)
            {
                MessageBox.Show($"{FindLocalizationResource("Lang-FailedToLoad")} {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                ex.ToSentry().TreatedBy(nameof(OfflineTaskViewModel.LazyLoad)).AttachExtraInfo(nameof(OfflineTaskViewModel), this).Submit();
            }
        }

        #region Commands
        public DependencyCommand NewTaskCommand { get; set; }
        private void NewTask(object parameters)
        {
            var a = new OfflineTaskDialog
            {
                DataContext = new OfflineTaskDialogViewModel()
            };
            a.ShowDialog();
            RefreshList();
        }

        public DependencyCommand CancelTaskCommand { get; set; }
        private async void CancelTask(object parameters)
        {
            var list = parameters as IList;
            if (list.Count > 0)
            {
                var cancellingTasks = list.Cast<OfflineTask>();
                var taskID = new List<string>(list.Count);
                foreach (var task in cancellingTasks)
                {
                    taskID.Add(task.TaskIdentity);
                }
                await Task.Run(() => OfflineDownloader.DeleteTask(taskID.ToArray()));
                RefreshList();
            }
        }


        public DependencyCommand RefreshListCommand { get; set; }
        private void RefreshList(object parameter = null)
        {
            taskEnumerator = GetTaskEnumeratorAsync().GetAsyncEnumerator();
            ObservableCollection.Clear();
            Task.Run(LazyLoad);
        }
        #endregion

        public OfflineTaskViewModel()
        {
            NewTaskCommand = new DependencyCommand(NewTask, DependencyCommand.AlwaysCan);
            CancelTaskCommand = new DependencyCommand(CancelTask, DependencyCommand.AlwaysCan);
            RefreshListCommand = new DependencyCommand(RefreshList, DependencyCommand.AlwaysCan);
            RefreshList();
        }
    }
}
