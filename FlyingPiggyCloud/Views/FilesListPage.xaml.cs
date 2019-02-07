using FlyingPiggyCloud.Models;
using FlyingPiggyCloud.UserControls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wangsu.WcsLib.Core;

namespace FlyingPiggyCloud.Views
{
    /// <summary>
    /// FilesListPage.xaml 的交互逻辑
    /// </summary>
    public partial class FilesListPage : Page
    {
        private FileList fileList;

        /// <summary>
        /// 地址栏
        /// </summary>
        private List<string> pathArray;

        private static List<FileListItem> Move = new List<FileListItem>();

        /// <summary>
        /// 指示Move列表中的项目需要复制还是剪切
        /// </summary>
        private static bool IsCopy;

        public bool IsStickButtonEnable => Move.Count == 0 ? false : true;

        /// <summary>
        /// 为后退按钮保存历史路径
        /// </summary>
        private Stack<string> previousPath;

        /// <summary>
        /// 为前进按钮保存历史路径
        /// </summary>
        private Stack<string> nextPath;

        /// <summary>
        /// 通过指定路径创建文件列表页，如果路径不存在将自动创建
        /// </summary>
        /// <param name="path"></param>
        public FilesListPage(string path)
        {
            fileList = new FileList("", path, true);
            InitializeComponent();
            FileListView.ItemsSource = fileList;
            CreatePathArray(path);
            previousPath = new Stack<string>();
            nextPath = new Stack<string>();
            Stick.IsEnabled = IsStickButtonEnable;
            LazyLoadEventHandler = LazyLoad;
        }

        private void CreatePathArray(string path)
        {
            if (path == "/")
                pathArray = new List<string>(new string[]
                {
                    "root"
                });
            else
                pathArray = new List<string>(System.Text.RegularExpressions.Regex.Split(path, "/", System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace));
            AddressBar.ItemsSource = pathArray;
        }

        private async void ListViewItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if(sender is ListViewItem listViewItem && listViewItem.DataContext is FileListItem a)
            {
                if (a.Type == 1)
                {
                    if (a.IsEnable)
                    {
                        previousPath.Push(fileList.CurrentPath);
                        nextPath.Clear();
                        try
                        {
                            await fileList.GetDirectoryByUUID(a.UUID);
                            
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(string.Format("打开失败，原因{0}", ex.Message));
                        }
                        finally
                        {
                            CreatePathArray(fileList.CurrentPath);
                        }
                    }
                    else
                    {
                        MessageBox.Show("这个项目被远程服务器锁定（可能的原因：正在被删除，或者其他客户端正在修改该项目）");
                        fileList.Refresh(this, new EventArgs());
                    }
                }
                //1000表示这是一个可以预览的视频
                else if (a.Preview == 1000)
                {
                    PreviewWindow previewWindow = new PreviewWindow(a);
                    try
                    {
                        previewWindow.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("预览请求失败：" + ex.Message);
                    }
                }
                //300表示这是一个可以预览的图片
                else if (a.Preview == 300)
                {
                    PreviewWindow previewWindow = new PreviewWindow(a);
                    try
                    {
                        previewWindow.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("预览请求失败：" + ex.Message);
                        previewWindow.Close();
                    }
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
                var downloadPathDialog = new System.Windows.Forms.FolderBrowserDialog
                {
                    Description = "请选择下载文件夹",
                    SelectedPath = Syroot.Windows.IO.KnownFolders.Downloads.Path
                };
                if(downloadPathDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
                {
                    string uuid = ((FileListItem)((Button)sender).DataContext).UUID;
                    Controllers.FileSystemMethods fileSystemMethods = new Controllers.FileSystemMethods(Properties.Settings.Default.BaseUri);
                    Controllers.Results.ResponesResult<Controllers.Results.FileSystem.FileMetaData> x = await fileSystemMethods.GetDetailsByUUID(uuid);
                    DownloadingListPage.NewDownloadTask(x.Result, downloadPathDialog.SelectedPath);
                }
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
            await ((FileListItem)x.DataContext).Remove();
            //System.Threading.Thread.Sleep(200);
            lock (fileList)
            {
                fileList.Remove((FileListItem)x.DataContext);
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

        private async void AddressBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is ListBox listBox)
            {
                var index = listBox.SelectedIndex;
                if (index != -1)
                {
                    listBox.SelectedIndex = -1;
                    string path = "";
                    if (pathArray[0] == "root")
                        path = "/";
                    else
                    {
                        for (int i = 1; i <= index; i++)
                        {
                            path = path + "/" + pathArray[i];
                        }
                    }
                    previousPath.Push(fileList.CurrentPath);
                    nextPath.Clear();
                    try
                    {
                        await fileList.GetDirectoryByPath(path);
                    }
                    catch (System.IO.DirectoryNotFoundException ex)
                    {
                        MessageBox.Show(string.Format("打开{0}文件夹失败，原因是：", pathArray[index], ex.Message));
                        await fileList.GetDirectoryByPath("/");
                    }
                    CreatePathArray(fileList.CurrentPath);
                }
            }
            
        }

        private async void Previous_Click(object sender, RoutedEventArgs e)
        {
            nextPath.Push(fileList.CurrentPath);
            if(previousPath.Count>0)
            {
                bool success;
                do
                {
                    try
                    {
                        string path = previousPath.Pop();
                        await fileList.GetDirectoryByPath(path);
                        success = true;
                        CreatePathArray(path);
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {
                        success = false;
                    }
                } while (!success && previousPath.Count != 0);
            }
            
        }

        private async void Next_Click(object sender, RoutedEventArgs e)
        {
            previousPath.Push(fileList.CurrentPath);
            if(nextPath.Count>0)
            {
                bool success;
                do
                {
                    try
                    {
                        string path = nextPath.Pop();
                        await fileList.GetDirectoryByPath(path);
                        success = true;
                        CreatePathArray(path);
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {
                        success = false;
                    }
                } while (!success && nextPath.Count != 0);
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Move?.Clear();
            var list = FileListView.SelectedItems;
            if(list!=null)
            {
                lock(Move)
                {
                    foreach (FileListItem item in list)
                    {
                        Move.Add(item);
                    }
                    IsCopy = true;
                    Stick.IsEnabled = IsStickButtonEnable;
                }
            }
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            Move?.Clear();
            var list = FileListView.SelectedItems;
            if (list != null)
            {
                lock (Move)
                {
                    foreach (FileListItem item in list)
                    {
                        Move.Add(item);
                    }
                    IsCopy = false;
                }
                Stick.IsEnabled = IsStickButtonEnable;
            }
        }

        private void Stick_Click(object sender, RoutedEventArgs e)
        {
            lock(Move)
            {
                if(Move.Count!=0)
                {
                    foreach (FileListItem item in Move)
                    {
                        if (IsCopy)
                        {
                            item.Copy(fileList.CurrentUUID);
                        }
                        else
                        {
                            item.Cut(fileList.CurrentUUID);
                        }
                        //if(!fileList.Contains(item))
                        //{
                        //    fileList.Add(item);
                        //}
                    }
                    Move.Clear();
                }
                Stick.IsEnabled = IsStickButtonEnable;
            }
        }

        #region 懒加载相关
        private void FileListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer viewer)
            {
                double bottomOffset = viewer.ExtentHeight - viewer.VerticalOffset - viewer.ViewportHeight;
                if (viewer.VerticalOffset > 0 && bottomOffset == 0)
                {
                    LazyLoadEventHandler?.Invoke(sender, e);
                }
            }
        }

        private ScrollChangedEventHandler LazyLoadEventHandler;

        private async void LazyLoad(object sender, ScrollChangedEventArgs e)
        {
            lock(LazyLoadEventHandler)
            {
                LazyLoadEventHandler = null;
            }
            //懒加载的业务代码
            await fileList.Lazyload();
            LazyLoadEventHandler = new ScrollChangedEventHandler(LazyLoad);
        }
        #endregion

        private void RenamableTextBox_Cancel(object sender, RoutedEventArgs e)
        {
            if(sender is RenamableTextBox renameableTextBox)
            {
                if(renameableTextBox.DataContext is FileListItem fileListItem)
                {
                    renameableTextBox.Text = fileListItem.Name;
                }
            }
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            var x = sender as MenuItem;
            if(x?.DataContext is FileListItem fileListItem)
            {
                fileListItem.IsRename = true;
            }
        }

        private void ListViewItem_Unselected(object sender, RoutedEventArgs e)
        {
            if(sender is ListViewItem listViewItem)
            {
                var f = listViewItem.DataContext as FileListItem;
                f.IsRename = false;
            }
        }

        private async void RenamableTextBox_Confirm(object sender, RoutedEventArgs e)
        {
            if(sender is RenamableTextBox renamableTextBox)
            {
                if(renamableTextBox.DataContext is FileListItem fileListItem)
                {
                    await fileListItem.Rename(renamableTextBox.Text);
                    fileListItem.IsRename = false;
                }
            }
        }
    }
}
