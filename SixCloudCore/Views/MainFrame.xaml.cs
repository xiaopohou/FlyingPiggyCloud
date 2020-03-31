using QingzhenyunApis.EntityModels;
using SixCloudCore.ViewModels;
using CustomControls.Controls;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using SourceChord.FluentWPF;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using SixCloudCore.Models;
using System.Linq;

namespace SixCloudCore.Views
{

    /// <summary>
    /// MainFrame.xaml 的交互逻辑
    /// </summary>
    public partial class MainFrame : AcrylicWindow
    {
        private static UserInformation recoveryInfo;

        public static Window Recovery()
        {
            if (recoveryInfo != null)
            {
                return new MainFrame(recoveryInfo);
            }
            else
            {
                return new LoginView();
            }
        }

        public MainFrame(UserInformation currentUser)
        {
            recoveryInfo = currentUser;
            InitializeComponent();
            MainFrameViewModel mainFrameViewModel = new MainFrameViewModel(currentUser);
            DataContext = mainFrameViewModel;
            ThreadPool.QueueUserWorkItem(async (_) =>
            {
                await mainFrameViewModel.FileVM.NavigateByPath("/");
            });
            LazyLoadEventHandler += LazyLoad;
        }

        private void UserInformationMenu_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void FileListContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //如果不是从文件列表Raise的事件，忽略
            if (e.OriginalSource is ListView)
            {
                if (e.AddedItems.Count == 0)
                {
                    InfoView.BeginAnimation(OpacityProperty, new DoubleAnimation(0d, new Duration(TimeSpan.FromMilliseconds(300d))));
                }
                else
                {
                    InfoView.BeginAnimation(OpacityProperty, new DoubleAnimation(0d, 1d, new Duration(TimeSpan.FromMilliseconds(300d))));
                }
            }
        }


        private void DownloadingTaskPause(object sender, ExecutedRoutedEventArgs e)
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

        private void DownloadingTaskCancel(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                IList list = (IList)e.Parameter;
                IEnumerable<DownloadingTaskViewModel> downloadingTasks = list.Cast<DownloadingTaskViewModel>();
                foreach (DownloadingTaskViewModel t in downloadingTasks.ToArray())
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
                LazyLoadEventHandler -= LazyLoad;
            }
            //懒加载的业务代码
            MainFrameViewModel vm = DataContext as MainFrameViewModel;
            await vm.OfflineTask.LazyLoad();
            LazyLoadEventHandler += LazyLoad;
        }

        private void UploadingTaskPause(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                IList list = (IList)e.Parameter;
                IEnumerable<UploadingTaskViewModel> downloadingTasks = list.Cast<UploadingTaskViewModel>();
                foreach (UploadingTaskViewModel t in downloadingTasks)
                {
                    if (t.Status == UploadingTaskViewModel.UploadStatus.Pause)
                    {
                        t.Start();
                    }
                    else if (t.Status == UploadingTaskViewModel.UploadStatus.Running)
                    {
                        t.Pause();
                    }
                }

            }
            catch (Exception)
            {

            }
        }

        private void UploadingTaskCancel(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                IList list = (IList)e.Parameter;
                IEnumerable<UploadingTaskViewModel> downloadingTasks = list.Cast<UploadingTaskViewModel>();
                foreach (var t in downloadingTasks)
                {
                    t.Stop(null);
                }

            }
            catch (Exception)
            {

            }
        }


    }
}
