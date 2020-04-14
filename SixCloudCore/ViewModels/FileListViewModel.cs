using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods.V3;
using SixCloudCore.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace SixCloudCore.ViewModels
{
    internal class FileListViewModel : ViewModelBase
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

        public ObservableCollection<FileListItemViewModel> FileList { get; private set; } = new ObservableCollection<FileListItemViewModel>();

        public string CurrentPath { get; set; }

        public string CurrentUUID { get; set; }

        #region FileListViewModelFunctions
        [DisallowNull]
        /// <summary>
        /// 保存LazyLoad状态的枚举器
        /// </summary>
        protected IAsyncEnumerable<FileMetaData> fileMetaDataEnumerator;

        /// <summary>
        /// 创建指定路径或uuid目录内容的枚举器
        /// </summary>
        /// <param name="path"></param>
        /// <param name="identity"></param>
        /// <exception cref="RequestFailedException">未能找到目录</exception>
        /// <returns></returns>
        public static async IAsyncEnumerable<FileMetaData> CreateFileListEnumerator(string path = null, string identity = null)
        {
            int start = 0;
            const int limit = 20;
            int count;
            do
            {
                var x = await FileSystem.GetDirectory(identity, path, start, limit);
                count = x.List.Count;
                foreach (var item in x.List)
                {
                    yield return item;
                }
                start += limit;
            } while (count == limit);
            yield break;
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
            OnPropertyChanged(nameof(PathArray));
        }

        public async void NavigateByUUIDAsync(string uuid)
        {
            previousPath.Push(CurrentPath);
            fileMetaDataEnumerator = CreateFileListEnumerator(identity: uuid);
            var directoryInfo = await FileSystem.GetDetailsByIdentity(uuid);
            CurrentPath = directoryInfo.Path;
            CurrentUUID = directoryInfo.UUID;
            CreatePathArray(CurrentPath);
            await LazyLoad();
            PreviousNavigateCommand.OnCanExecutedChanged(this, new EventArgs());
        }

        public async Task NavigateByPath(string path)
        {
            previousPath.Push(CurrentPath);
            fileMetaDataEnumerator = CreateFileListEnumerator(path);
            var directoryInfo = (await FileSystem.GetDirectory(path: path, start: 0, limit: 1)).DictionaryInformation;
            CurrentPath = directoryInfo.Path;
            CurrentUUID = directoryInfo.UUID;
            CreatePathArray(CurrentPath);
            await LazyLoad();
            PreviousNavigateCommand.OnCanExecutedChanged(this, new EventArgs());
        }

        public async void NavigateByPathAsync(string path)
        {
            await NavigateByPath(path);
        }

        public async Task LazyLoad()
        {
            try
            {
                int count = 0;
                await foreach (var item in fileMetaDataEnumerator)
                {
                    count++;
                    App.Current.Dispatcher.Invoke(() => FileList.Add(new FileListItemViewModel(this, item)));
                    if (count >= 20)
                    {
                        break;
                    }
                }
            }
            catch (RequestFailedException ex)
            {
                MessageBox.Show($"加载目录失败，由于{ex.Message}");
            }
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
                string path = previousPath.Pop();
                await NavigateByPath(path);
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
        private readonly Stack<string> previousPath = new Stack<string>();
        #endregion

        #region NextNavigate
        public DependencyCommand NextNavigateCommand { get; private set; }

        private async void NextNavigate(object parameter)
        {
            previousPath.Push(CurrentPath);
            if (nextPath.Count > 0)
            {
                string path = nextPath.Pop();
                await NavigateByPath(path);
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
        private readonly Stack<string> nextPath = new Stack<string>();
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
                try
                {
                    var x = await Task.Run(() => FileSystem.CreatDirectory(FolderName, CurrentUUID));

                }
                catch (RequestFailedException ex)
                {
                    MessageBox.Show("创建失败：" + ex.Message);
                    return;
                }

                NavigateByUUIDAsync(CurrentUUID);
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
                     await FileSystem.Copy(copyList, CurrentUUID);
                 }
                 else if (CutList != null && CutList.Length > 0)
                 {
                     string[] cutList = CutList;
                     CutList = null;
                     StickCommand.OnCanExecutedChanged(this, new EventArgs());
                     await FileSystem.Move(cutList, CurrentUUID);
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

        private async void Delete(object parameter)
        {
            if (parameter is IList selectedItems)
            {
                List<string> list = new List<string>(selectedItems.Count);
                foreach (FileListItemViewModel a in selectedItems)
                {
                    list.Add(a.UUID);
                }
                await FileSystem.Remove(list.ToArray());
                NavigateByUUIDAsync(CurrentUUID);
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
            OpenFileDialog commonOpenFileDialog = new OpenFileDialog
            {
                Multiselect = true
            };

            if (commonOpenFileDialog.ShowDialog() == true)
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
        #endregion

        #region UploadFolderCommand
        public DependencyCommand UploadFolderCommand { get; private set; }
        public async void UploadFolder(object parameter)
        {
            using (FolderBrowserDialog commonOpenFileDialog = new FolderBrowserDialog())
            {
                if (commonOpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (Directory.Exists(commonOpenFileDialog.SelectedPath))
                    {
                        await FoundFolder(new DirectoryInfo(commonOpenFileDialog.SelectedPath), CurrentPath);
                    }
                }
            }

            static async Task FoundFolder(DirectoryInfo path, string parentPath)
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
                if (str.Length > length)
                {
                    return $"{str.Substring(0, 14)}...{str.Substring(str.Length - 3)}";
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
            if (value is bool directory)
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