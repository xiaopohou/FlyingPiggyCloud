using FlyingPiggyCloud.Controllers.Results.FileSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FlyingPiggyCloud.ViewModels
{
    internal class FileList : ObservableCollection<FileListItem>
    {
        private static Controllers.FileSystemMethods FileSystemMethods = new Controllers.FileSystemMethods(Properties.Settings.Default.BaseUri);

        /// <summary>
        /// 当前目录的UUID
        /// </summary>
        public string CurrentUUID { get; private set;}

        /// <summary>
        /// 当前路径
        /// </summary>
        public string CurrentPath { get; set; }

        /// <summary>
        /// 把指定UUID的目录加载到当前列表
        /// </summary>
        /// <param name="UUID"></param>
        public async void GetDirectoryByUUID(string UUID)
        {
            Clear();
            List<FileMetaData> items = new List<FileMetaData>();
            int Page = 1;
            do
            {
                PageResponseResult x = await FileSystemMethods.GetDirectory(UUID, "", Page);
                if (!x.Success)
                {
                    throw new Exception(x.Message);
                }

                CurrentUUID = x.Result.DictionaryInformation.UUID;
                CurrentPath = x.Result.DictionaryInformation.Path;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentPath"));
                if (Page == x.Result.TotalPage)
                {
                    Page = 0;
                }
                else
                {
                    Page++;
                }
                items.AddRange(x.Result.List);
            } while (Page != 0);

            foreach (FileMetaData a in items)
            {
                Add(new FileListItem(a));
            }
        }

        /// <summary>
        /// 把指定Path的目录加载到当前列表
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="IsAutoCreating">当路径不存在时是否自动新建该文件夹</param>
        public async void GetDirectoryByPath(string Path, bool IsAutoCreating = false)
        {
            Clear();
            List<FileMetaData> items = new List<FileMetaData>();
            int Page = 1;
            do
            {
                PageResponseResult x = await FileSystemMethods.GetDirectory("", Path, Page);
                if (!x.Success && IsAutoCreating)
                {
                    await FileSystemMethods.CreatDirectory("", "", Path);
                    x = await FileSystemMethods.GetDirectory("", Path, Page);
                }
                if (x.Success)
                {
                    CurrentUUID = x.Result.DictionaryInformation.UUID;
                    CurrentPath = x.Result.DictionaryInformation.Path;
                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentPath"));
                    if (Page == x.Result.TotalPage)
                    {
                        Page = 0;
                    }
                    else
                    {
                        Page++;
                    }
                    items.AddRange(x.Result.List);
                }
            } while (Page != 0);

            foreach (FileMetaData a in items)
            {
                Add(new FileListItem(a));
            }
        }

        /// <summary>
        /// 指定文件夹名称在当前路径新建文件夹
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="IsAutoRename">指示若存在同名文件夹，是否自动重命名</param>
        /// <returns></returns>
        public async Task<bool> NewFolder(string Name, bool IsAutoRename = false)
        {
            foreach (FileListItem a in Items)
            {
                if (a.Name == Name)
                {
                    if (IsAutoRename)
                    {
                        Name = Name + DateTime.Now.ToString();
                    }
                    else
                    {
                        return false;
                    }

                    break;
                }
            }
            GetMetaDataResponseResult x = await FileSystemMethods.CreatDirectory(Name, CurrentUUID);
            if (!x.Success)
            {
                throw new Exception(x.Message);
            }

            if (x.Success)
            {
                Add(new FileListItem(x.Result));
            }
            return x.Success;
        }

        /// <summary>
        /// 刷新当前列表
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        public void Refresh(object Sender, EventArgs e)
        {
            GetDirectoryByUUID(CurrentUUID);
        }

        /// <summary>
        /// 根据UUID或Path获取指定文件夹的内容，如果均空则返回根目录
        /// </summary>
        /// <param name="UUID"></param>
        /// <param name="Path"></param>
        public FileList(string UUID = "", string Path = "/", bool IsAutoCreating = false)
        {
            if (UUID != "")
            {
                GetDirectoryByUUID(UUID);
            }
            else
            {
                GetDirectoryByPath(Path,IsAutoCreating);
            }
        }
    }
}
