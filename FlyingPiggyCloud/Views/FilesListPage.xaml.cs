using FlyingPiggyCloud.Models;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Wangsu.WcsLib.Core;

namespace FlyingPiggyCloud.Views
{
    /// <summary>
    /// FilesListPage.xaml 的交互逻辑
    /// </summary>
    public partial class FilesListPage : Page
    {
        private FileList fileList;

        private List<string> PathArray;

        /// <summary>
        /// 通过指定路径创建文件列表页，如果路径不存在将自动创建
        /// </summary>
        /// <param name="Path"></param>
        public FilesListPage(string Path)
        {
            fileList = new FileList("", Path, true);
            InitializeComponent();
            FileListView.ItemsSource = fileList;
            PathArray = new List<string>(Path.Split(new string[] { "/" }, System.StringSplitOptions.None));
            if (PathArray[PathArray.Count - 1] == "")
            {
                PathArray.RemoveAt(PathArray.Count - 1);
            }

            AddressBar.ItemsSource = PathArray;
        }

        private void ListViewItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            FileListItem a = (Models.FileListItem)((ListViewItem)sender).DataContext;
            if (a.Type == 1)
            {
                if (a.IsEnable)
                {
                    fileList.GetDirectoryByUUID(((Models.FileListItem)((ListViewItem)sender).DataContext).UUID);
                }
                else
                {
                    MessageBox.Show("这个项目被远程服务器锁定（可能的原因：正在被删除，或者其他客户端正在修改该项目）");
                    fileList.Refresh(this, new System.EventArgs());
                }
            }
        }

        private async void NewFolderButton_Click(object sender, RoutedEventArgs e)
        {
            string NewFolderName = "新文件夹";
            await fileList.NewFolder(NewFolderName, true);
            fileList.Refresh(sender, e);
        }

        private async void DownloadBotton_Click(object sender, RoutedEventArgs e)
        {
            if (((FileListItem)((Button)sender).DataContext).Type == 0)
            {
                string uuid = ((FileListItem)((Button)sender).DataContext).UUID;
                Controllers.FileSystemMethods fileSystemMethods = new Controllers.FileSystemMethods(Properties.Settings.Default.BaseUri);
                Controllers.Results.ResponesResult<Controllers.Results.FileSystem.FileMetaData> x = await fileSystemMethods.GetDetailsByUUID(uuid);
                DownloadingListPage.NewDownloadTask(x.Result);
            }
        }

        private void MoreBotton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.ContextMenu.DataContext = btn.DataContext;
            btn.ContextMenu.IsOpen = true;
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem x = (MenuItem)sender;
            //x.IsEnabled = false;
            await ((Models.FileListItem)x.DataContext).Remove();
            //System.Threading.Thread.Sleep(200);
            lock (fileList)
            {
                fileList.Refresh(this, new System.EventArgs());
            }
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog().GetValueOrDefault())
            {
                UploadingListPage.NewUploadTask(openFileDialog, fileList.CurrentUUID);
            }
        }
    }
}
