using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloud.Core.ViewModels
{
    public class RecoveryBoxViewModel : ViewModelBase
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
            Application.Current.Dispatcher.Invoke(() => Refresh());
        }
        #endregion

        #region Delete
        public DependencyCommand DeleteCommand { get; private set; }

        private async void Delete(object parameter)
        {
            if (parameter is IList selectedItems)
            {
                var list = new List<string>(selectedItems.Count);
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
                var list = new List<string>(selectedItems.Count);
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

        private IAsyncEnumerator<RecoveryBoxItem> recoveryBoxItemsEnumerator;
        private async IAsyncEnumerable<RecoveryBoxItem> CreateRecoveryBoxListEnumerator()
        {
            var skip = 0;
            const int limit = 20;
            int count;
            do
            {
                var x = await RecoveryBox.GetList(skip, limit);
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
                    if (await recoveryBoxItemsEnumerator.MoveNextAsync())
                    {
                        Application.Current.Dispatcher.Invoke(() => RecoveryList.Add(recoveryBoxItemsEnumerator.Current));
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
        }

        public void Refresh()
        {
            recoveryBoxItemsEnumerator = CreateRecoveryBoxListEnumerator().GetAsyncEnumerator();
            RecoveryList.Clear();
            Task.Run(LazyLoad);
        }

    }
}
