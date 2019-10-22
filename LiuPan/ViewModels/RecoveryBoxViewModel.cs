using SixCloud.Controllers;
using SixCloud.Models;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace SixCloud.ViewModels
{
    internal class RecoveryBoxViewModel : ViewModelBase
    {
        private static readonly RecoveryBox recoveryBox = new RecoveryBox();

        public RecoveryBoxViewModel()
        {
            EmptyCommand = new AsyncCommand(Empty, DependencyCommand.AlwaysCan);
            DeleteCommand = new DependencyCommand(Delete, DependencyCommand.AlwaysCan);
            RecoveryCommand = new DependencyCommand(Recovery, DependencyCommand.AlwaysCan);
            recoveryBoxItemsEnumerator = LoadList().GetEnumerator();
        }

        #region Empty
        public AsyncCommand EmptyCommand { get; private set; }
        private void Empty(object parameter)
        {
            recoveryBox.Empty();
            App.Current.Dispatcher.Invoke(() => Refresh());
        }
        #endregion

        #region Delete
        public DependencyCommand DeleteCommand { get; private set; }

        private void Delete(object parameter)
        {
            if (parameter is IList selectedItems)
            {
                List<string> list = new List<string>(selectedItems.Count);
                foreach (RecoveryBoxItem a in selectedItems)
                {
                    list.Add(a.Identity);
                }
                recoveryBox.Delete(list.ToArray());
                Refresh();
            }
        }
        #endregion

        #region Recovery
        public DependencyCommand RecoveryCommand { get; private set; }

        private void Recovery(object parameter)
        {
            if (parameter is IList selectedItems)
            {
                List<string> list = new List<string>(selectedItems.Count);
                foreach (RecoveryBoxItem a in selectedItems)
                {
                    list.Add(a.Identity);
                }
                recoveryBox.Restore(list.ToArray());
                Refresh();
            }
        }
        #endregion

        public ObservableCollection<RecoveryBoxItem> RecoveryList { get; private set; } = new ObservableCollection<RecoveryBoxItem>();

        private int currentPage = 0;
        private IEnumerator<RecoveryBoxItem[]> recoveryBoxItemsEnumerator;
        private IEnumerable<RecoveryBoxItem[]> LoadList()
        {
            int totalPage;
            do
            {
                GenericResult<RecoveryBoxPage> x = recoveryBox.GetList(++currentPage);
                totalPage = x.Result.TotalPage;
                yield return x.Result.List;
            } while (currentPage < totalPage);
            yield break;
        }
        public void LazyLoad()
        {
            if (recoveryBoxItemsEnumerator.MoveNext())
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
                recoveryBoxItemsEnumerator = LoadList().GetEnumerator();
                RecoveryList.Clear();
                LazyLoad();
            }
        }

    }
}
