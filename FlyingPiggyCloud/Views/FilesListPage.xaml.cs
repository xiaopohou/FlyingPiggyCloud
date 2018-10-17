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
            fileList.GetDirectoryByUUID(((ViewModels.FileListItem)((ListViewItem)sender).DataContext).UUID);
                
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
    }

}
