using FlyingPiggyCloud.Controllers;
using FlyingPiggyCloud.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace FlyingPiggyCloud.Views
{
    /// <summary>
    /// CompletedListPage.xaml 的交互逻辑
    /// </summary>
    public partial class CompletedListPage : Page
    {
        private static ObservableCollection<ICompletedTask> CompletedTasks = new ObservableCollection<ICompletedTask>();

        internal static void CompletedTasksAddRange(List<DownloadTask> completedTasks)
        {
            foreach (DownloadTask a in completedTasks)
            {
                if (!CompletedTasks.Contains(a))
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        lock (CompletedTasks)
                        {
                            CompletedTasks.Add(a);
                        }
                    });
                }
            }
        }

        internal static void CompletedTasksAdd(ICompletedTask completedTask)
        {
            CompletedTasks.Add(completedTask);
        }

        public CompletedListPage()
        {
            InitializeComponent();
            CompletedList.ItemsSource = CompletedTasks;
        }

        private void UploadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CompletedTasks.Clear();
        }

        private void More_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button btn = sender as Button;
            ICompletedTask task = btn.DataContext as ICompletedTask;
            if (task.TaskType == TaskTypeEnum.Download)
            {
                btn.ContextMenu.DataContext = btn.DataContext;
                btn.ContextMenu.IsOpen = true;
            }
        }

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem.DataContext is DownloadTask downloadTask)
            {
                System.Diagnostics.Process.Start(downloadTask.FilePath);
            }
        }

        private void MenuItem_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem.DataContext is DownloadTask downloadTask)
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe")
                {
                    Arguments = "/e,/select," + downloadTask.FilePath
                };
                System.Diagnostics.Process.Start(psi);
            }

        }
    }
}
