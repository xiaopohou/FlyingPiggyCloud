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
        }

        public FilesListPage(string Path)
        {
            fileList = new ViewModels.FileList("", Path);
            InitializeComponent();
        }

        private async void NewFolderButton_Click(object sender, RoutedEventArgs e)
        {
            string NewFolderName = "新文件夹";
            int Count = 0;
            while (!(await fileList.NewFolder(NewFolderName)))
            {
                Count++;
                NewFolderName = NewFolderName + string.Format("（{0}）", Count);
            }
            fileList.Refresh(sender, e);
        }
    }
}
