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
        private static ObservableCollection<ViewModels.DownloadTask> CompletedTasks = new ObservableCollection<ViewModels.DownloadTask>();

        internal static void CompletedTasksAddRange(List<ViewModels.DownloadTask> completedTasks)
        {
            lock (CompletedTasks)
            {
                foreach (ViewModels.DownloadTask a in completedTasks)
                {
                    CompletedTasks.Add(a);
                }
            }
        }

        public CompletedListPage()
        {
            InitializeComponent();
        }
    }
}
