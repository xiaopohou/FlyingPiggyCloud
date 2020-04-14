using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods.V3;
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

        //protected override async Task GetFileListByPath(string path)
        //{
        //    async IAsyncEnumerable<FileMetaData[]> GetFileListAsync()
        //    {
        //        int currentPage = 0;
        //        int totalPage = 0;
        //        FileListPage x;
        //        do
        //        {
        //            try
        //            {
        //                x = await FileSystem.GetDirectoryAsPage(path: path, page: ++currentPage);
        //            }
        //            catch (RequestFailedException ex)
        //            {
        //                throw new DirectoryNotFoundException(ex.Message);
        //            }

        //            if (x.DictionaryInformation != null)
        //            {
        //                totalPage = x.FileListPageInfo.TotalPage;
        //                CurrentPath = x.DictionaryInformation.Path;
        //                CurrentUUID = x.DictionaryInformation.UUID;
        //                CreatePathArray(CurrentPath);
        //                yield return x.List;
        //            }
        //            else
        //            {
        //                throw new DirectoryNotFoundException();
        //            }

        //        } while (currentPage < totalPage);
        //        yield break;
        //    }

        //    App.Current.Dispatcher.Invoke(() => FileList.Clear());
        //    fileMetaDataEnumerator = GetFileListAsync().GetAsyncEnumerator();
        //    await fileMetaDataEnumerator.MoveNextAsync();
        //    App.Current.Dispatcher.Invoke(() =>
        //    {
        //        foreach (FileMetaData a in fileMetaDataEnumerator.Current)
        //        {
        //            if (Mode == Mode.PathSelector && !a.Directory)
        //            {
        //                continue;
        //            }
        //            FileList.Add(new FileListItemViewModel(this, a));
        //        }
        //    });
        //}

        //protected override async Task GetFileListByUUID(string uuid)
        //{
        //    {
        //        async IAsyncEnumerable<FileMetaData[]> GetFileListAsync()
        //        {
        //            int currentPage = 0;
        //            int totalPage;
        //            FileListPage x;
        //            do
        //            {
        //                try
        //                {
        //                    x = await FileSystem.GetDirectoryAsPage(uuid, page: ++currentPage);
        //                }
        //                catch (RequestFailedException ex)
        //                {
        //                    throw new DirectoryNotFoundException(ex.Message);
        //                }

        //                if (x.DictionaryInformation != null)
        //                {
        //                    totalPage = x.FileListPageInfo.TotalPage;
        //                    CurrentPath = x.DictionaryInformation.Path;
        //                    CurrentUUID = x.DictionaryInformation.UUID;
        //                    CreatePathArray(CurrentPath);
        //                    yield return x.List;
        //                }
        //                else
        //                {
        //                    throw new DirectoryNotFoundException();
        //                }

        //            } while (currentPage < totalPage);
        //            yield break;
        //        }


        //        Application.Current.Dispatcher.Invoke(() => FileList.Clear());
        //        fileMetaDataEnumerator = GetFileListAsync().GetAsyncEnumerator();
        //        await fileMetaDataEnumerator.MoveNextAsync();
        //        Application.Current.Dispatcher.Invoke(() =>
        //        {
        //            foreach (FileMetaData a in fileMetaDataEnumerator.Current)
        //            {
        //                if (Mode == Mode.PathSelector && !a.Directory)
        //                {
        //                    continue;
        //                }
        //                FileList.Add(new FileListItemViewModel(this, a));
        //            }
        //        });
        //    }
        //}

        public DependencyCommand NavigateCommand { get; set; }
        private void Navigate(object parameter)
        {
            FileListItemViewModel selectObject = (FileListItemViewModel)parameter;
            NavigateByUUIDAsync(selectObject.UUID);
        }

        public FileGridViewModel()
        {
            NavigateCommand = new DependencyCommand(Navigate, DependencyCommand.AlwaysCan);
            NavigateByPathAsync("/");
        }
    }
}