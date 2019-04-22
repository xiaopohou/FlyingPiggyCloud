using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloud.ViewModels
{
    internal class UploadingListViewModel : ViewModelBase
    {
        public ObservableCollection<UploadingTaskViewModel> ObservableCollection => _observableCollection;
        private static readonly ObservableCollection<UploadingTaskViewModel> _observableCollection = new ObservableCollection<UploadingTaskViewModel>();

        public static async Task NewTask(FileListViewModel targetList, string path)
        {
            if (Directory.Exists(path))
            {

            }
            else if (File.Exists(path))
            {
                UploadingFileViewModel task = await Task.Run(() => new UploadingFileViewModel(targetList.CurrentUUID, path));
                _observableCollection.Add(task);
                task.UploadCompleted += (sender, e) =>
                {
                    _observableCollection.Remove(task);
                    UploadedListViewModel.NewTask(task);
                };
            }
            else
            {
                MessageBox.Show("由于找不到对象，6盘未能创建任务", "失败", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
    }
}
