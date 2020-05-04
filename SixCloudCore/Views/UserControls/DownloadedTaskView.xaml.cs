﻿using System.Windows.Controls;

namespace SixCloudCore.Views.UserControls
{
    /// <summary>
    /// DownloaderTaskView.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadedTaskView : UserControl
    {
        public DownloadedTaskView()
        {
            InitializeComponent();
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
