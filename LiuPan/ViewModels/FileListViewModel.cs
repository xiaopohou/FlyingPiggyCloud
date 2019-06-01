using Microsoft.WindowsAPICodePack.Dialogs;
using SixCloud.Models;
using SixCloud.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SixCloud.ViewModels
{
    internal class FileListViewModel : FileSystemViewModel
    {
        private bool canNavigate = true;
        public static string[] CopyList
        {
            get => s_copyList;
            set
            {
                lock (_copyListSyncRoot)
                {
                    s_copyList = value;
                }
            }
        }
        private static readonly object _copyListSyncRoot = new object();

        public static string[] CutList
        {
            get => _cutList;
            set
            {
                lock (_cutListSyncRoot)
                {
                    _cutList = value;
                }
            }
        }
        private static readonly object _cutListSyncRoot = new object();

        public FileListItemViewModel FileListItemInformation
        {
            get
            {
                if (SelectedIndex == -1 || SelectedIndex > FileList.Count - 1)
                {
                    return null;
                }
                else
                {
                    return FileList[SelectedIndex];
                }
            }
        }
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged(nameof(FileListItemInformation));
            }
        }

        static FileListViewModel()
        {

        }

        public List<string> PathArray { get; set; } = new List<string>();

        public ObservableCollection<FileListItemViewModel> FileList { get; set; } = new ObservableCollection<FileListItemViewModel>();

        public string CurrentPath { get; set; }

        public string CurrentUUID { get; set; }

        #region FileListViewModelFunctions
        /// <summary>
        /// 保存LazyLoad状态的枚举器
        /// </summary>
        protected IEnumerator<FileMetaData[]> fileMetaDataEnumerator;

        public async void NavigateByUUID(string uuid)
        {
            previousPath.Push(CurrentPath);
            PreviousNavigateCommand.OnCanExecutedChanged(this, new EventArgs());
            await GetFileListByUUID(uuid);
        }

        public async Task NavigateByPath(string path, bool autoCreate = false)
        {
            if (!canNavigate)
            {
                return;
            }
            canNavigate = false;
            try
            {
                previousPath.Push(CurrentPath);
                PreviousNavigateCommand.OnCanExecutedChanged(this, new EventArgs());
                try
                {
                    await GetFileListByPath(path);
                }
                catch (DirectoryNotFoundException ex)
                {
                    if (autoCreate)
                    {
                        GenericResult<FileMetaData> x = await Task.Run(() => fileSystem.CreatDirectory(Path: path));
                        if (x.Success)
                        {
                            await GetFileListByPath(x.Result.Path);
                        }
                        else
                        {
                            MessageBox.Show($"打开{path}文件夹失败，SixCloud表示它既不能找到也无法创建您想要的对象。服务器返回：{x.Message}", "找不到对象", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"打开{path}文件夹失败，SixCloud表示它找不到这个目录。服务器返回：{ex.Message}", "错误");
                    }
                }
            }
            finally
            {
                canNavigate = true;
            }
        }

        public async void NavigateByPathAsync(string path, bool autoCreate = false)
        {
            await NavigateByPath(path, autoCreate);
        }

        protected virtual async Task GetFileListByPath(string path)
        {
            IEnumerable<FileMetaData[]> GetFileList()
            {
                int currentPage = 0;
                int totalPage;
                do
                {
                    GenericResult<FileListPage> x = fileSystem.GetDirectory(path: path, page: ++currentPage);
                    if (x.Success && x.Result.DictionaryInformation != null)
                    {
                        totalPage = x.Result.TotalPage;
                        CurrentPath = x.Result.DictionaryInformation.Path;
                        CurrentUUID = x.Result.DictionaryInformation.UUID;
                        CreatePathArray(CurrentPath);
                        yield return x.Result.List;
                    }
                    else if(x.Success)
                    {
                        break;
                    }
                    else
                    {
                        throw new DirectoryNotFoundException(x.Message);
                    }
                } while (currentPage < totalPage);
                yield break;
            }

            App.Current.Dispatcher.Invoke(() => FileList.Clear());
            await Task.Run(() =>
            {
                fileMetaDataEnumerator = GetFileList().GetEnumerator();
                fileMetaDataEnumerator.MoveNext();
            });
            App.Current.Dispatcher.Invoke(() =>
            {
                if(fileMetaDataEnumerator.Current==null)
                {
                    return;
                }
                foreach (FileMetaData a in fileMetaDataEnumerator.Current)
                {
                    FileList.Add(new FileListItemViewModel(this, a));
                }
            });
        }

        protected virtual async Task GetFileListByUUID(string uuid)
        {
            IEnumerable<FileMetaData[]> GetFileList()
            {
                int currentPage = 0;
                int totalPage;
                do
                {
                    GenericResult<FileListPage> x = fileSystem.GetDirectory(uuid, page: ++currentPage);
                    if (x.Success)
                    {
                        totalPage = x.Result.TotalPage;
                        CurrentPath = x.Result.DictionaryInformation.Path;
                        CurrentUUID = x.Result.DictionaryInformation.UUID;
                        CreatePathArray(CurrentPath);
                        yield return x.Result.List;
                    }
                    else
                    {
                        throw new DirectoryNotFoundException(x.Message);
                    }
                } while (currentPage < totalPage);
                yield break;
            }


            App.Current.Dispatcher.Invoke(() => FileList.Clear());
            await Task.Run(() =>
            {
                fileMetaDataEnumerator = GetFileList().GetEnumerator();
                fileMetaDataEnumerator.MoveNext();
            });
            App.Current.Dispatcher.Invoke(() =>
            {
                foreach (FileMetaData a in fileMetaDataEnumerator.Current)
                {
                    FileList.Add(new FileListItemViewModel(this, a));
                }
            });
        }

        public void LazyLoad()
        {
            if (fileMetaDataEnumerator != null)
            {
                try
                {
                    if (fileMetaDataEnumerator.MoveNext())
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            foreach (FileMetaData a in fileMetaDataEnumerator.Current)
                            {
                                FileList.Add(new FileListItemViewModel(this, a));
                            }
                        });
                    }
                    else
                    {
                        fileMetaDataEnumerator = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "加载文件列表失败");
                }
            }

        }

        protected void CreatePathArray(string path)
        {
            if (path == "/")
            {
                PathArray = new List<string>(new string[]
                {
                    "home"
                });
            }
            else
            {
                List<string> array = new List<string>(System.Text.RegularExpressions.Regex.Split(path, "/", System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace))
                {
                    //array.Insert(0, "home");
                    [0] = "home"
                };
                PathArray = array;
            }
            OnPropertyChanged("PathArray");
        }
        #endregion

        #region NavigationCommand

        #region PreviousNavigate
        public DependencyCommand PreviousNavigateCommand { get; private set; }

        private async void PreviousNavigate(object parameter)
        {
            nextPath.Push(CurrentPath);
            if (previousPath.Count > 0)
            {
                bool success;
                do
                {
                    try
                    {
                        string path = previousPath.Pop();
                        await GetFileListByPath(path);
                        success = true;
                        //CreatePathArray(path);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        success = false;
                    }
                } while (!success && previousPath.Count != 0);
            }
            PreviousNavigateCommand.OnCanExecutedChanged(this, new EventArgs());
            NextNavigateCommand.OnCanExecutedChanged(this, new EventArgs());
        }

        private bool CanPreviousNavigate(object parameter)
        {
            return previousPath.Count != 0 ? true : false;
        }

        /// <summary>
        /// 为后退按钮保存历史路径
        /// </summary>
        private Stack<string> previousPath = new Stack<string>();
        #endregion

        #region NextNavigate
        public DependencyCommand NextNavigateCommand { get; private set; }

        private async void NextNavigate(object parameter)
        {
            previousPath.Push(CurrentPath);
            if (nextPath.Count > 0)
            {
                bool success;
                do
                {
                    try
                    {
                        string path = nextPath.Pop();
                        await GetFileListByPath(path);
                        success = true;
                        //CreatePathArray(path);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        success = false;
                    }
                } while (!success && nextPath.Count != 0);
            }
            PreviousNavigateCommand.OnCanExecutedChanged(this, new EventArgs());
            NextNavigateCommand.OnCanExecutedChanged(this, new EventArgs());
        }

        private bool CanNextNavigate(object parameter)
        {
            return nextPath.Count != 0 ? true : false;
        }

        /// <summary>
        /// 为前进按钮保存历史路径
        /// </summary>
        private Stack<string> nextPath = new Stack<string>();
        private static string[] s_copyList;
        private static string[] _cutList;
        private int _selectedIndex;
        #endregion
        #endregion

        #region FileOperationCommand
        #region NewFolderCommand
        public DependencyCommand NewFolderCommand { get; private set; }

        private async void NewFolder(object parameter)
        {
            if (TextInputDialog.Show(out string FolderName, "请输入新文件夹的名字", "新建文件夹") && !FolderName.Contains("/"))
            {
                GenericResult<FileMetaData> x = await Task.Run(() => fileSystem.CreatDirectory(FolderName, CurrentUUID));
                if (!x.Success)
                {
                    MessageBox.Show("创建失败：" + x.Message);
                    return;
                }
                NavigateByUUID(CurrentUUID);
            }
        }
        #endregion

        #region Stick
        public DependencyCommand StickCommand { get; private set; }

        private void Stick(object parameter)
        {
            new LoadingView(parameter as Window, async () =>
             {
                 if (CopyList != null && CopyList.Length > 0)
                 {
                     string[] copyList = CopyList;
                     CopyList = null;
                     StickCommand.OnCanExecutedChanged(this, new EventArgs());
                     fileSystem.Copy(copyList, CurrentUUID);
                 }
                 else if (CutList != null && CutList.Length > 0)
                 {
                     string[] cutList = CutList;
                     CutList = null;
                     StickCommand.OnCanExecutedChanged(this, new EventArgs());
                     fileSystem.Move(cutList, CurrentUUID);
                 }
                 else
                 {
                     CutList = null;
                     CopyList = null;
                     StickCommand.OnCanExecutedChanged(this, new EventArgs());
                 }
                 await NavigateByPath(CurrentPath);
             }, "正在与服务器py，SixCloud处理中").Show();

        }

        private bool CanStick(object parameter)
        {
            if ((CopyList != null && CopyList.Length > 0) || (CutList != null && CutList.Length > 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Cut
        public DependencyCommand CutCommand { get; private set; }

        private void Cut(object parameter)
        {
            if (parameter is IList selectedItems)
            {
                List<string> list = new List<string>(selectedItems.Count);
                foreach (FileListItemViewModel a in selectedItems)
                {
                    list.Add(a.UUID);
                }
                CutList = list.ToArray();
                CopyList = null;
                StickCommand.OnCanExecutedChanged(this, new EventArgs());
            }
        }
        #endregion

        #region Delete
        public DependencyCommand DeleteCommand { get; private set; }

        private void Delete(object parameter)
        {
            if (parameter is IList selectedItems)
            {
                List<string> list = new List<string>(selectedItems.Count);
                foreach (FileListItemViewModel a in selectedItems)
                {
                    list.Add(a.UUID);
                }
                fileSystem.Remove(list.ToArray());
                NavigateByUUID(CurrentUUID);
            }
        }

        #endregion

        #region Copy
        public DependencyCommand CopyCommand { get; private set; }

        private void Copy(object parameter)
        {
            if (parameter is IList selectedItems)
            {
                List<string> list = new List<string>(selectedItems.Count);
                foreach (FileListItemViewModel a in selectedItems)
                {
                    list.Add(a.UUID);
                }
                CopyList = list.ToArray();
                CutList = null;
                StickCommand.OnCanExecutedChanged(this, new EventArgs());
            }
        }
        #endregion

        #region UploadFileCommand
        public DependencyCommand UploadFileCommand { get; private set; }
        public async void UploadFile(object parameter)
        {
            using (CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = false,
                Multiselect = true
            })
            {
                if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    foreach (string p in commonOpenFileDialog.FileNames)
                    {
                        if (File.Exists(p))
                        {
                            await UploadingListViewModel.NewTask(this, p);
                        }
                    }
                }
            }
        }
        #endregion

        #region UploadFolderCommand
        public DependencyCommand UploadFolderCommand { get; private set; }
        public async void UploadFolder(object parameter)
        {
            using (CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false
            })
            {
                if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    if (Directory.Exists(commonOpenFileDialog.FileName))
                    {
                        await FoundFolder(new DirectoryInfo(commonOpenFileDialog.FileName), CurrentPath);
                    }
                }
            }

            async Task FoundFolder(DirectoryInfo path, string parentPath)
            {
                string currentPath = parentPath == "/" ? $"/{path.Name}" : $"{parentPath}/{path.Name}";
                if (path.Exists)
                {
                    FileInfo[] list = path.GetFiles();
                    if (list.Length != 0)
                    {
                        foreach (FileInfo a in list)
                        {
                            if (a.Exists)
                            {
                                await UploadingListViewModel.NewTask(currentPath, a.FullName);
                            }
                        }
                    }
                    DirectoryInfo[] directorys = path.GetDirectories();
                    if (directorys.Length != 0)
                    {
                        foreach (DirectoryInfo a in directorys)
                        {
                            if (a.Exists)
                            {
                                await FoundFolder(a, currentPath);
                            }
                        }
                    }

                }
            }
        }
        #endregion

        #endregion

        public FileListViewModel()
        {
            NextNavigateCommand = new DependencyCommand(NextNavigate, CanNextNavigate);
            PreviousNavigateCommand = new DependencyCommand(PreviousNavigate, CanPreviousNavigate);
            StickCommand = new DependencyCommand(Stick, CanStick);
            CutCommand = new DependencyCommand(Cut, DependencyCommand.AlwaysCan);
            DeleteCommand = new DependencyCommand(Delete, DependencyCommand.AlwaysCan);
            CopyCommand = new DependencyCommand(Copy, DependencyCommand.AlwaysCan);
            NewFolderCommand = new DependencyCommand(NewFolder, DependencyCommand.AlwaysCan);
            UploadFileCommand = new DependencyCommand(UploadFile, DependencyCommand.AlwaysCan);
            UploadFolderCommand = new DependencyCommand(UploadFolder, DependencyCommand.AlwaysCan);
        }
    }

    public class TooLongStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                int length = (parameter as int?) ?? 20;
                if(str.Length>length)
                {
                    return $"{str.Substring(0,14)}...{str.Substring(str.Length-3)}";
                }
                else
                {
                    return str;
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HidingSizeinfoForDirectoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool directory)
            {
                return directory ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}