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

        /// <summary>
        /// 从后台恢复前台窗口
        /// </summary>
        public static void Recovery()
        {
            if (recoveryInfo != null)
            {
                new MainFrame(recoveryInfo).Show();
            }
            else
            {
                new LoginWebViewModel();
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


    }
}
