using FlyingPiggyCloud.Controllers.Results.FileSystem;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FlyingPiggyCloud.ViewModels
{
    internal class FileListItem:INotifyPropertyChanged
    {
        private FileMetaData MetaData;

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Path => MetaData.Path;

        public string UUID => MetaData.UUID;

        public bool IsChecked { get; set; }

        public async Task<bool> Remove()
        {
            Controllers.FileSystemMethods fileSystemMethods = new Controllers.FileSystemMethods(Properties.Settings.Default.BaseUri);
            return (await fileSystemMethods.Remove(MetaData)).Result;
        }

        public async Task<FileMetaData> Rename(string NewName)
        {
            Controllers.FileSystemMethods fileSystemMethods = new Controllers.FileSystemMethods(Properties.Settings.Default.BaseUri);
            MetaData = (await fileSystemMethods.Rename(MetaData, NewName)).Result;
            //OnPropertyChanged("Path");
            //OnPropertyChanged("UUID");
            //OnPropertyChanged("MTime");
            OnPropertyChanged("Name");
            return MetaData;
        }

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
        public string Name => MetaData.Name;

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
