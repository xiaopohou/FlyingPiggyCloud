using SixCloud.Models;
using SixCloud.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace SixCloud.Views.UserControls
{
    /// <summary>
    /// TransferListView.xaml 的交互逻辑
    /// </summary>
    public partial class TransferListView : UserControl
    {
        public TransferListView()
        {
            InitializeComponent();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                IList list = (IList)e.Parameter;
                IEnumerable<DownloadingTaskViewModel> downloadingTasks = list.Cast<DownloadingTaskViewModel>();
                foreach (DownloadingTaskViewModel t in downloadingTasks)
                {
                    if (t.Status == DownloadTask.TaskStatus.Pause)
                    {
                        t.Start();
                    }
                    else if (t.Status == DownloadTask.TaskStatus.Running)
                    {
                        t.Pause();
                    }
                }

            }
            catch (Exception)
            {

            }
        }

        private void CommandBinding_Executed_1(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                IList list = (IList)e.Parameter;
                IEnumerable<DownloadingTaskViewModel> downloadingTasks = list.Cast<DownloadingTaskViewModel>();
                foreach (DownloadingTaskViewModel t in downloadingTasks)
                {
                    t.Stop();
                }

            }
            catch (Exception)
            {

            }
        }

        private void DownloadedList_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ListBox list = sender as ListBox;
            if (list.SelectedItem == null)
            {
                e.Handled = true;
            }
            else
            {
                list.ContextMenu.DataContext = list.SelectedItem;
            }
        }

        private void OfflineList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer viewer)
            {
                double bottomOffset = (viewer.ExtentHeight - viewer.VerticalOffset - viewer.ViewportHeight) / viewer.ExtentHeight;
                if (viewer.VerticalOffset > 0 && bottomOffset < 0.3)
                {
                    LazyLoadEventHandler?.Invoke(sender, e);
                }
            }
        }

        private ScrollChangedEventHandler LazyLoadEventHandler;

        private async void LazyLoad(object sender, ScrollChangedEventArgs e)
        {
            lock (LazyLoadEventHandler)
            {
                LazyLoadEventHandler = null;
            }
            //懒加载的业务代码
            OfflineTaskViewModel vm = DataContext as OfflineTaskViewModel;
            await Task.Run(() => vm.LazyLoad());
            LazyLoadEventHandler = new ScrollChangedEventHandler(LazyLoad);
        }

    }
}
