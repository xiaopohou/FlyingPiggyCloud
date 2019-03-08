using SixCloud.Controllers;
using SixCloud.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace SixCloud.ViewModels
{
    internal class FileListViewModel : INotifyPropertyChanged
    {
        private readonly FileSystem fileSystem = new FileSystem();

        public List<string> PathArray { get; set; }

        public ObservableCollection<FileListItemViewModel> FileList { get; set; }

        public string CurrentPath { get; set; }

        #region FileListViewModelFunctions
        /// <summary>
        /// 保存LazyLoad状态的枚举器
        /// </summary>
        private IEnumerator<FileMetaData[]> fileMetaDataEnumerator;

        private void GetFileList(string path)
        {
            IEnumerable<FileMetaData[]> GetFileList()
            {
                int currentPage;
                int totalPage;
                do
                {
                    GenericResult<FileListPage> x = fileSystem.GetDirectory("", path);
                    if (x.Success)
                    {
                        currentPage = x.Result.Page;
                        totalPage = x.Result.TotalPage;
                        yield return x.Result.List;
                    }
                    else
                    {
                        throw new DirectoryNotFoundException(x.Message);
                    }
                } while (currentPage < totalPage);
                yield break;
            }
            fileMetaDataEnumerator = GetFileList().GetEnumerator();
            App.Current.Dispatcher.Invoke(() =>
            {
                foreach (FileMetaData a in fileMetaDataEnumerator.Current)
                {
                    FileList.Add(new FileListItemViewModel(a));
                }
            });
        }

        private void LazyLoad()
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
                                FileList.Add(new FileListItemViewModel(a));
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
                    "root"
                });
            }
            else
            {
                PathArray = new List<string>(System.Text.RegularExpressions.Regex.Split(path, "/", System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace));
            }
            OnPropertyChanged("PathArray");
        }

        #endregion

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
                        await Task.Run(() =>
                        {
                            GetFileList(path);
                        });
                        success = true;
                        CreatePathArray(path);
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
        private Stack<string> previousPath;
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
                        await Task.Run(() =>
                        {
                            GetFileList(path);
                        });
                        success = true;
                        CreatePathArray(path);
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
        private Stack<string> nextPath;
        #endregion

        public FileListViewModel()
        {
            NextNavigateCommand = new DependencyCommand(NextNavigate, CanNextNavigate);
            PreviousNavigateCommand = new DependencyCommand(PreviousNavigate, CanPreviousNavigate);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal class FileListItemViewModel
    {
        public FileListItemViewModel(FileMetaData fileMetaData)
        {

        }
    }
}
