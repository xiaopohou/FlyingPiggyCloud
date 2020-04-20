using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
