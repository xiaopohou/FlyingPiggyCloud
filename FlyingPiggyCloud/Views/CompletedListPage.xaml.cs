using FlyingPiggyCloud.Controllers;
using FlyingPiggyCloud.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;

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
            Button btn = (Button)sender;
            btn.ContextMenu.DataContext = btn.DataContext;
            btn.ContextMenu.IsOpen = true;
        }

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ICompletedTask completedTask = (ICompletedTask)((MenuItem)sender).DataContext;
            System.Diagnostics.Process.Start(completedTask.FilePath);
        }

        private void MenuItem_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            ICompletedTask completedTask = (ICompletedTask)((MenuItem)sender).DataContext;
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            psi.Arguments = "/e,/select," + completedTask.FilePath;
            System.Diagnostics.Process.Start(psi);
        }
    }
}
