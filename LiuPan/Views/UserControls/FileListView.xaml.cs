using SixCloud.ViewModels;
using System.Text;
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
                if (lvItem.DataContext is FileListItemViewModel fileListItem && DataContext is FileListViewModel dataContext)
                {
                    if (fileListItem.Directory)
                    {
                        dataContext.NavigateByUUID(fileListItem.UUID);
                    }
                    else if (fileListItem.Preview)
                    {
                        fileListItem.NewPreView();
                    }
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
            await vm.LazyLoad();
            LazyLoadEventHandler = new ScrollChangedEventHandler(LazyLoad);
        }

        private void AddressBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && DataContext is FileListViewModel viewmodel)
            {
                int i = listBox.SelectedIndex;
                if (i == 0)
                {
                    viewmodel.NavigateByPathAsync("/");
                }
                else if (i != -1)
                {
                    string[] pathArray = new string[i];
                    viewmodel.PathArray.CopyTo(1, pathArray, 0, i);
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (string path in pathArray)
                    {
                        stringBuilder.Append("/");
                        stringBuilder.Append(path);
                    }
                    viewmodel.NavigateByPathAsync(stringBuilder.ToString());
                }
                listBox.SelectedIndex = -1;
            }
        }
    }
}
