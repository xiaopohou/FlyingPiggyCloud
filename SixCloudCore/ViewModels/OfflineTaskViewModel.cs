using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods;
using QingzhenyunApis.Methods.V3;
using SixCloudCore.Views;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloudCore.ViewModels
{
    internal class OfflineTaskViewModel : ViewModelBase
    {
        public ObservableCollection<OfflineTask> ObservableCollection { get; private set; } = new ObservableCollection<OfflineTask>();

        private IAsyncEnumerator<OfflineTask> taskEnumerator;
        private async IAsyncEnumerable<OfflineTask> GetTaskEnumeratorAsync()
        {
            int skip = 0;
            const int limit = 20;
            int count;
            do
            {
                var x = await OfflineDownloader.GetList(skip, limit);
                count = x.List.Count;
                foreach (var item in x.List)
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
                for (int count = 0; count < 20; count++)
                {
                    if (await taskEnumerator.MoveNextAsync())
                    {
                        App.Current.Dispatcher.Invoke(() => ObservableCollection.Add(taskEnumerator.Current));
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (RequestFailedException ex)
            {
                MessageBox.Show($"加载目录失败，由于{ex.Message}");
            }
        }

        #region Commands
        public DependencyCommand NewTaskCommand { get; set; }
        private void NewTask(object parameters)
        {
            OfflineTaskDialog a = new OfflineTaskDialog
            {
                DataContext = new OfflineTaskDialogViewModel()
            };
            a.ShowDialog();
            RefreshList();
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
