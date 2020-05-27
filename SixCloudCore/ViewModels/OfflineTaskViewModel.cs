using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using SixCloudCore.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

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
                OfflineTaskList x = await OfflineDownloader.GetList(skip, limit);
                count = x.List.Count;
                foreach (OfflineTask item in x.List)
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
            catch (InvalidOperationException ex)
            {
                ex.ToSentry().TreatedBy(nameof(OfflineTaskViewModel.LazyLoad)).AttachExtraInfo(nameof(OfflineTaskViewModel), this).Submit();
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

    internal class TaskStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OfflineTask offlineTask)
            {
                if (offlineTask.Progress == 100)
                {
                    return "已完成";
                }
                else
                {
                    return offlineTask.Status switch
                    {
                        100 => "排队中",
                        1000 => "下载完成",
                        1301 => "正在下载",
                        300 => "正在下载",
                        _ => "重试中"
                    };
                }
            }
            else if (value is TransferTaskStatus transferItem)
            {
                return transferItem switch
                {
                    TransferTaskStatus.Completed => "已完成",
                    TransferTaskStatus.Pause => "暂停",
                    TransferTaskStatus.Running => "进行中",
                    TransferTaskStatus.Stop => "停止",
                    _ => "未定义的状态枚举"
                };
            }
            else
            {
                return "未定义的状态枚举";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
