using SixCloud.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SixCloud.Views.UserControls
{
    /// <summary>
    /// FileListView.xaml 的交互逻辑
    /// </summary>
    public partial class FileListView : UserControl
    {
        public FileListView()
        {
            InitializeComponent();
            LazyLoadEventHandler += LazyLoad;
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ListViewItem lvItem)
            {
                if (lvItem.DataContext is FileListItemViewModel fileListItem)
                {
                    var dataContext = DataContext as FileListViewModel;
                    dataContext?.Navigate(fileListItem.UUID);
                }
            }
        }

        private void FileList_ScrollChanged(object sender, ScrollChangedEventArgs e)
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
            FileListViewModel vm = DataContext as FileListViewModel;
            await Task.Run(() => vm.LazyLoad());
            LazyLoadEventHandler = new ScrollChangedEventHandler(LazyLoad);
        }
    }
}
