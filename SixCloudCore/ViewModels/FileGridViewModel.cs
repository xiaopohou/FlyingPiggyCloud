using QingzhenyunApis.EntityModels;
using SixCloudCore.Models;
using SixCloudCore.Views.UserControls;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloudCore.ViewModels
{
    internal class FileGridViewModel : FileListViewModel
    {
        public Mode Mode { get; set; } = Mode.FileListContainer;

        public object SelectObject { get; set; }

        protected override async Task GetFileListByPath(string path)
        {
            async IAsyncEnumerable<FileMetaData[]> GetFileListAsync()
            {
                int currentPage = 0;
                int totalPage;
                do
                {
                    GenericResult<FileListPage> x = await fileSystem.GetDirectory(path: path, page: ++currentPage);
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
            fileMetaDataEnumerator = GetFileListAsync().GetAsyncEnumerator();
            await fileMetaDataEnumerator.MoveNextAsync();
            App.Current.Dispatcher.Invoke(() =>
            {
                foreach (FileMetaData a in fileMetaDataEnumerator.Current)
                {
                    if (Mode == Mode.PathSelector && !a.Directory)
                    {
                        continue;
                    }
                    FileList.Add(new FileListItemViewModel(this, a));
                }
            });
        }

        protected override async Task GetFileListByUUID(string uuid)
        {
            {
                async IAsyncEnumerable<FileMetaData[]> GetFileListAsync()
                {
                    int currentPage = 0;
                    int totalPage;
                    do
                    {
                        GenericResult<FileListPage> x = await fileSystem.GetDirectory(uuid, page: ++currentPage);
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


                Application.Current.Dispatcher.Invoke(() => FileList.Clear());
                fileMetaDataEnumerator = GetFileListAsync().GetAsyncEnumerator();
                await fileMetaDataEnumerator.MoveNextAsync();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (FileMetaData a in fileMetaDataEnumerator.Current)
                    {
                        if (Mode == Mode.PathSelector && !a.Directory)
                        {
                            continue;
                        }
                        FileList.Add(new FileListItemViewModel(this, a));
                    }
                });
            }
        }

        public DependencyCommand NavigateCommand { get; set; }
        private async void Navigate(object parameter)
        {
            var selectObject = (FileListItemViewModel)parameter;
            await GetFileListByUUID(selectObject.UUID);
            await LazyLoad();
        }

        public FileGridViewModel()
        {
            NavigateCommand = new DependencyCommand(Navigate, DependencyCommand.AlwaysCan);
            Task.Run(async () =>
            {
                await GetFileListByPath("/");
            });
        }
    }
}