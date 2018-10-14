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
        private string CurrentUUID;

        /// <summary>
        /// 当前路径
        /// </summary>
        public string CurrentPath { get; set; }

        /// <summary>
        /// 把指定UUID的目录加载到当前列表
        /// </summary>
        /// <param name="UUID"></param>
        private async void GetDirectoryByUUID(string UUID)
        {
            Clear();
            List<FileMetaData> items = new List<FileMetaData>();
            int Page = 1;
            do
            {
                PageResponseResult x = await FileSystemMethods.GetDirectory(UUID, "", Page);
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
        private async void GetDirectoryByPath(string Path)
        {
            Clear();
            List<FileMetaData> items = new List<FileMetaData>();
            int Page = 1;
            do
            {
                PageResponseResult x = await FileSystemMethods.GetDirectory("", Path, Page);
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
        /// 指定文件夹名称在当前路径新建文件夹
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public async Task<bool> NewFolder(string Name)
        {
            var x = await FileSystemMethods.CreatDirectory(Name, CurrentUUID);
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
        public FileList(string UUID="",string Path="/")
        {
            if (UUID != "")
                GetDirectoryByUUID(UUID);
            else
                GetDirectoryByPath(Path);
        }
    }

    internal class FileListItem
    {
        private readonly FileMetaData MetaData;

        public string Path => MetaData.Path;

        public string UUID => MetaData.UUID;

        public bool IsChecked { get; set; }

        public string MTime => MetaData.Mtime.ToString();

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get
            {
                switch (MetaData.Type)
                {
                    case 0:
                        return "";
                    case 1:
                        return "";
                    default:
                        return "";
                }
            }
        }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName => MetaData.Name;

        /// <summary>
        /// 文件大小
        /// </summary>
        public string Size => Controllers.ConverterToolKits.SizeCalculator(MetaData.Size);

        /// <summary>
        /// 下载地址
        /// </summary>
        public string DownloadAddress => MetaData.DownloadAddress;

        public FileListItem(FileMetaData fileMetaData)
        {
            MetaData = fileMetaData;
        }
    }
}
