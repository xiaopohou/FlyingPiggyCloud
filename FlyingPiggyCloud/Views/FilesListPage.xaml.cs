using System.Text.RegularExpressions;
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

        private string[] Path => fileList.CurrentPath.Split("/".ToCharArray());

        public FilesListPage()
        {
            fileList = new ViewModels.FileList();
            InitializeComponent();
            FileListView.ItemsSource = fileList;
        }

        public FilesListPage(string Path)
        {
            fileList = new ViewModels.FileList("", Path);
            InitializeComponent();
            FileListView.ItemsSource = fileList;
        }

        private async void NewFolderButton_Click(object sender, RoutedEventArgs e)
        {
            string NewFolderName = "新文件夹";
            //int Count = 0;
            //while (!(await fileList.NewFolder(NewFolderName)))
            //{
            //    Count++;
            //    NewFolderName = "新文件夹" + string.Format("（{0}）", Count);
            //}
            await fileList.NewFolder(NewFolderName);
            fileList.Refresh(sender, e);

        }
    }
}
