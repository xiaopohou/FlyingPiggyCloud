using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods;
using QingzhenyunApis.Methods.V3;
using SixCloudCore.Controllers;
using SixCloudCore.Models;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloudCore.ViewModels
{
    internal class RecoveryBoxViewModel : ViewModelBase
    {
        public RecoveryBoxViewModel()
        {
            EmptyCommand = new DependencyCommand(Empty, DependencyCommand.AlwaysCan);
            DeleteCommand = new DependencyCommand(Delete, DependencyCommand.AlwaysCan);
            RecoveryCommand = new DependencyCommand(Recovery, DependencyCommand.AlwaysCan);
            Refresh();
        }

        #region Empty
        public DependencyCommand EmptyCommand { get; private set; }
        private async void Empty(object parameter)
        {
            await RecoveryBox.Empty();
            App.Current.Dispatcher.Invoke(() => Refresh());
        }
        #endregion

        #region Delete
        public DependencyCommand DeleteCommand { get; private set; }

        private async void Delete(object parameter)
        {
            if (parameter is IList selectedItems)
            {
                List<string> list = new List<string>(selectedItems.Count);
                foreach (RecoveryBoxItem a in selectedItems)
                {
                    list.Add(a.Identity);
                }
                await RecoveryBox.Delete(list.ToArray());
                Refresh();
            }
        }
        #endregion

        #region Recovery
        public DependencyCommand RecoveryCommand { get; private set; }

        private async void Recovery(object parameter)
        {
            if (parameter is IList selectedItems)
            {
                List<string> list = new List<string>(selectedItems.Count);
                foreach (RecoveryBoxItem a in selectedItems)
                {
                    list.Add(a.Identity);
                }
                await RecoveryBox.Restore(list.ToArray());
                Refresh();
            }
        }
        #endregion

        public ObservableCollection<RecoveryBoxItem> RecoveryList { get; private set; } = new ObservableCollection<RecoveryBoxItem>();

        private IAsyncEnumerable<RecoveryBoxItem> recoveryBoxItemsEnumerator;
        private async IAsyncEnumerable<RecoveryBoxItem> CreateRecoveryBoxListEnumerator()
        {
            int start = 0;
            const int limit = 20;
            int count;
            do
            {
                var x = await RecoveryBox.GetList(start, limit);
                count = x.List.Count;
                foreach (var item in x.List)
                {
                    yield return item;
                }
                start += limit;
            } while (count == limit);
            yield break;
        }

        public async Task LazyLoad()
        {
            try
            {
                int count = 0;
                await foreach (var item in recoveryBoxItemsEnumerator)
                {
                    count++;
                    App.Current.Dispatcher.Invoke(() => RecoveryList.Add(item));
                    if (count >= 20)
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

        public void Refresh()
        {
            recoveryBoxItemsEnumerator = CreateRecoveryBoxListEnumerator();
            RecoveryList.Clear();
            Task.Run(LazyLoad);
        }

    }
}
