using QingzhenyunApis.EntityModels;
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
        private readonly OfflineDownloader downloader = new OfflineDownloader();

        private IAsyncEnumerator<OfflineTask[]> listEnumerator;
        private async IAsyncEnumerable<OfflineTask[]> GetListEnumeratorAsync()
        {
            int currentPage = 0;
            int totalPage;
            do
            {
                GenericResult<OfflineTaskList> x = await downloader.GetList(++currentPage);
                totalPage = x.Result.TotalPage;
                yield return x.Result.List;
            } while (currentPage < totalPage);
            yield break;
        }

        public async Task LazyLoad()
        {
            listEnumerator ??= GetListEnumeratorAsync().GetAsyncEnumerator();

            if (await listEnumerator.MoveNextAsync())
            {
                foreach (OfflineTask a in listEnumerator.Current)
                {
                    ObservableCollection.Add(a);
                }

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
                    taskID.Add(task.Identity);
                }
                await Task.Run(() => downloader.DeleteTask(taskID.ToArray()));
                RefreshList();
            }
        }


        public DependencyCommand RefreshListCommand { get; set; }
        private async void RefreshList(object parameter = null)
        {
#warning 这里有严重的性能问题
            ObservableCollection.Clear();
            listEnumerator = null;
            await LazyLoad();
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
