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
        private static readonly RecoveryBox recoveryBox = new RecoveryBox();

        public RecoveryBoxViewModel()
        {
            EmptyCommand = new DependencyCommand(Empty, DependencyCommand.AlwaysCan);
            DeleteCommand = new DependencyCommand(Delete, DependencyCommand.AlwaysCan);
            RecoveryCommand = new DependencyCommand(Recovery, DependencyCommand.AlwaysCan);
            recoveryBoxItemsEnumerator = LoadList().GetAsyncEnumerator();
        }

        #region Empty
        public DependencyCommand EmptyCommand { get; private set; }
        private async void Empty(object parameter)
        {
            await recoveryBox.Empty();
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
                await recoveryBox.Delete(list.ToArray());
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
                await recoveryBox.Restore(list.ToArray());
                Refresh();
            }
        }
        #endregion

        public ObservableCollection<RecoveryBoxItem> RecoveryList { get; private set; } = new ObservableCollection<RecoveryBoxItem>();

        private int currentPage = 0;
        private IAsyncEnumerator<RecoveryBoxItem[]> recoveryBoxItemsEnumerator;
        private async IAsyncEnumerable<RecoveryBoxItem[]> LoadList()
        {
            int totalPage;
            do
            {
                GenericResult<RecoveryBoxPage> x = await recoveryBox.GetList(++currentPage);
                totalPage = x.Result.TotalPage;
                yield return x.Result.List;
            } while (currentPage < totalPage);
            yield break;
        }
        public async Task LazyLoad()
        {
            if (await recoveryBoxItemsEnumerator.MoveNextAsync())
            {
                RecoveryBoxItem[] x = recoveryBoxItemsEnumerator.Current;
                foreach (RecoveryBoxItem a in x)
                {
                    Application.Current.Dispatcher.Invoke(() => RecoveryList.Add(a));
                }
            }
        }

        public void Refresh()
        {
            lock(RecoveryList)
            {
                currentPage = 0;
                recoveryBoxItemsEnumerator = LoadList().GetAsyncEnumerator();
                RecoveryList.Clear();
                LazyLoad().Wait();
            }
        }

    }
}
