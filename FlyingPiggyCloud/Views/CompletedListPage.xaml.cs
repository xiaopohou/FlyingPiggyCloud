using FlyingPiggyCloud.ViewModels;
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
        private static ObservableCollection<DownloadTask> CompletedTasks = new ObservableCollection<DownloadTask>();

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
    }
}
