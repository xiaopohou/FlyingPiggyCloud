using Microsoft.WindowsAPICodePack.Dialogs;
using SixCloud.Models;
using SixCloud.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloud.ViewModels
{
    internal class FileListViewModel : FileSystemViewModel
    {
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
        private IEnumerator<FileMetaData[]> fileMetaDataEnumerator;

        public async void NavigateByUUID(string uuid)
        {
            previousPath.Push(CurrentPath);
            PreviousNavigateCommand.OnCanExecutedChanged(this, new EventArgs());
            await GetFileListByUUID(uuid);
        }

        public async Task NavigateByPath(string path, bool autoCreate = false)
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

        public async void NavigateByPathAsync(string path, bool autoCreate = false)
        {
            await NavigateByPath(path, autoCreate);
        }

        private async Task GetFileListByPath(string path)
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

        private async Task GetFileListByUUID(string uuid)
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
                    System.Windows.MessageBox.Show(ex.Message, "加载文件列表失败");
                }
            }

        }

        private void CreatePathArray(string path)
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
                }
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
             }, "正在与服务器py，SixCloud处理中").ShowDialog();

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

        private bool CanCut(object parameter)
        {
            return true;
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

        private bool CanCopy(object parameter)
        {
            return true;
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
            CutCommand = new DependencyCommand(Cut, CanCut);
            CopyCommand = new DependencyCommand(Copy, CanCopy);
            NewFolderCommand = new DependencyCommand(NewFolder, DependencyCommand.AlwaysCan);
            UploadFileCommand = new DependencyCommand(UploadFile, DependencyCommand.AlwaysCan);
            UploadFolderCommand = new DependencyCommand(UploadFolder, DependencyCommand.AlwaysCan);
        }
    }
}
