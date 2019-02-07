using FlyingPiggyCloud.Controllers.Results.FileSystem;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FlyingPiggyCloud.Models
{
    internal class FileListItem : INotifyPropertyChanged
    {
        private string IcomUriConverter()
        {
            const string UriBase = @"pack://application:,,,/Resources/FileMetaIcons/";
            if (MetaData.Type == 1)
            {
                return UriBase + "archiver.png";
            }
            else
            {
                string extension = System.IO.Path.GetExtension(MetaData.Name);
                switch (extension)
                {
                    //case ".xls":
                    //case ".xlsx":
                    //case ".xlsm":
                    //case ".csv":
                    //    return UriBase + "xls.png";
                    //case ".avi":
                    //    return UriBase + "avi.png";
                    ////case ".mov":
                    //case ".mkv":
                    //    return UriBase + "mkv.png";
                    //case ".mp4":
                    //    return UriBase + "mp4.png";
                    //case ".mpg":
                    //case ".mpeg":
                    //    return UriBase + "mpg.png";
                    ////case ".wmv":
                    //case ".rm":
                    //case ".rmvb":
                    //    return UriBase + "rmvb.png";
                    //case ".mp3":
                    //    return UriBase + "mp3.png";
                    ////case ".wma":
                    //case ".wav":
                    //    return UriBase + "wav.png";
                    ////case ".ogg":
                    ////case ".aac":
                    ////case ".mid":
                    ////case ".flac":
                    ////case ".ape":
                    ////    return UriBase + "Music.png";
                    //case ".pdf":
                    //    return UriBase + "pdf.png";
                    //case ".txt":
                    //    return UriBase + "txt.png";
                    //case ".doc":
                    //case ".docx":
                    //case ".dotx":
                    //case ".dotm":
                    //    return UriBase + "doc.png";
                    //case ".dll":
                    //    return UriBase + "dll.png";
                    //case ".exe":
                    //    return UriBase + "exe.png";
                    //case ".gif":
                    //    return UriBase + "gif.png";
                    //case ".html":
                    //case ".htm":
                    //    return UriBase + "html.png";
                    //case ".jpg":
                    //    return UriBase + "jpg.png";
                    //case ".png":
                    //    return UriBase + "png.png";
                    //case ".ppt":
                    //case ".pptx":
                    //    return UriBase + "ppt.png";
                    //case ".psd":
                    //case ".psb":
                    //    return UriBase + "psd.png";
                    //case ".swf":
                    //    return UriBase + "swf.png";
                    //case ".zip":
                    //case ".7z":
                    //case ".rar":
                    //case ".gz":
                    //    return UriBase + "zip.png";
                    case ".ai":
                        return UriBase + "file-ai.png";
                    case ".mp3":
                    case ".aac":
                    case ".ape":
                    case ".flac":
                    case ".wav":
                    case ".wma":
                    case ".ogg":
                    case ".mid":
                        return UriBase + "file-audio.png";
                    case ".doc":
                    case ".docx":
                        return UriBase + "file-doc.png";
                    case ".eps":
                        return UriBase + "file-eps.png";
                    case ".gif":
                        return UriBase + "file-gif.png";
                    case ".jpg":
                    case ".jpeg":
                        return UriBase + "file-jpg.png";
                    case ".png":
                        return UriBase + "file-png.png";
                    case ".ppt":
                    case ".pptx":
                        return UriBase + "file-ppt.png";
                    case ".psd":
                        return UriBase + "file-psd.png";
                    case ".svg":
                        return UriBase + "file-svg.png";
                    case ".xls":
                    case ".xlsx":
                    case ".csv":
                        return UriBase + "file-xls.png";
                    case ".mov":
                    case ".rm":
                    case ".rmvb":
                    case ".mp4":
                    case ".mkv":
                    case ".wmv":
                    case ".avi":
                        return UriBase + "file-video.png";

                    default:
                        return UriBase + "file.png";
                }
            }
        }

        private FileMetaData MetaData;

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ImageSource Icon => new BitmapImage(new Uri(IcomUriConverter()));

        public string Path => MetaData.Path;

        private bool isRename = false;
        public bool IsRename
        {
            get => isRename;
            set
            {
                isRename = value;
                OnPropertyChanged("IsRename");
            }
        }

        public string UUID => MetaData.UUID;

        public int Type => MetaData.Type;

        public int Preview => MetaData.Preview;

        public bool IsChecked { get; set; }

        public async Task<bool> Remove()
        {
            Controllers.FileSystemMethods fileSystemMethods = new Controllers.FileSystemMethods(Properties.Settings.Default.BaseUri);
            return (await fileSystemMethods.Remove(MetaData)).Result;
        }

        public async Task<FileMetaData> Rename(string NewName)
        {
            Controllers.FileSystemMethods fileSystemMethods = new Controllers.FileSystemMethods(Properties.Settings.Default.BaseUri);
            //MetaData = (await fileSystemMethods.Rename(MetaData, NewName)).Result;
            await fileSystemMethods.Rename(MetaData, NewName);
            //OnPropertyChanged("Path");
            //OnPropertyChanged("UUID");
            //OnPropertyChanged("MTime");
            MetaData.Name = NewName;
            OnPropertyChanged("Name");
            return MetaData;
        }

        public async void Copy(string newParentUUID)
        {
            Controllers.FileSystemMethods fileSystemMethods = new Controllers.FileSystemMethods(Properties.Settings.Default.BaseUri);
            await fileSystemMethods.Copy(MetaData, newParentUUID);
        }

        public async void Cut(string newParentUUID)
        {
            Controllers.FileSystemMethods fileSystemMethods = new Controllers.FileSystemMethods(Properties.Settings.Default.BaseUri);
            await fileSystemMethods.Move(MetaData, newParentUUID);
        }


        private string UnixTimeStampConverter(long UnixTimeStamp)
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(UnixTimeStamp).ToString("yyyy/MM/dd HH:mm:ss");
        }

        public string MTime => UnixTimeStampConverter(MetaData.Mtime);

        /// <summary>
        /// 文件名
        /// </summary>
        public string Name => MetaData.Name;

        public bool IsEnable => !MetaData.Locking;

        /// <summary>
        /// 文件大小
        /// </summary>
        public string Size => Controllers.Calculators.SizeCalculator(MetaData.Size);

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
