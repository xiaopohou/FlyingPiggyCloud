using System.Windows;
using System.Windows.Controls;

namespace FlyingPiggyCloud.Views
{
    /// <summary>
    /// FilesListPage.xaml 的交互逻辑
    /// </summary>
    public partial class FilesListPage : Page
    {
        private ViewModels.FileList fileList;

        //public FilesListPage()
        //{
        //    fileList = new ViewModels.FileList();
        //    InitializeComponent();
        //    FileListView.ItemsSource = fileList;
        //}

        //public ObservableCollection<string> CurrentPath;

        /// <summary>
        /// 通过指定路径创建文件列表页，如果路径不存在将自动创建
        /// </summary>
        /// <param name="Path"></param>
        public FilesListPage(string Path)
        {
            fileList = new ViewModels.FileList("", Path,true);
            InitializeComponent();
            FileListView.ItemsSource = fileList;
            System.Collections.Generic.List<string> x = new System.Collections.Generic.List<string>(Path.Split(new string[] { "/" }, System.StringSplitOptions.None));
            if (x[x.Count - 1] == "")
                x.RemoveAt(x.Count - 1);
            AdressBar.ItemsSource = x;
        }

        private void ListViewItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            var a = (ViewModels.FileListItem)((ListViewItem)sender).DataContext;
            if (a.IsEnable)
            {
                fileList.GetDirectoryByUUID(((ViewModels.FileListItem)((ListViewItem)sender).DataContext).UUID);
            }
            else
            {
                MessageBox.Show("这个项目被远程服务器锁定（可能的原因：正在被删除，或者其他客户端正在修改该项目）");
                fileList.Refresh(this, new System.EventArgs());
            }
        }

        private async void NewFolderButton_Click(object sender, RoutedEventArgs e)
        {
            string NewFolderName = "新文件夹";
            await fileList.NewFolder(NewFolderName,true);
            fileList.Refresh(sender, e);
        }

        private void DownloadBotton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(((ViewModels.FileListItem)((Button)sender).DataContext).DownloadAddress);
        }

        private void MoreBotton_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            btn.ContextMenu.DataContext = btn.DataContext;
            btn.ContextMenu.IsOpen = true;
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var x = (MenuItem)sender;
            //x.IsEnabled = false;
            await ((ViewModels.FileListItem)x.DataContext).Remove();
            //System.Threading.Thread.Sleep(200);
            lock(fileList)
            {
                fileList.Refresh(this, new System.EventArgs());
            }
        }
    }

}
