using FlyingPiggyCloud.Controllers.Results.FileSystem;
using Syroot.Windows.IO;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace FlyingPiggyCloud.Models
{
    internal class DownloadTask : FlyingAria2c.DownloadTask, INotifyPropertyChanged
    {
        private readonly FileMetaData fileMetaData;

        public string FileName => fileMetaData.Name;

        public string Size => Controllers.ConverterToolKits.SizeCalculator(fileMetaData.Size);

        public new async Task RefreshStatus()
        {
            await base.RefreshStatus();
            OnPropertyChanged("Status");
            OnPropertyChanged("Progress");
            OnPropertyChanged("DownloadSpeed");
            System.Threading.Thread.Sleep(500);
        }

        public DownloadTask(FileMetaData fileMetaData) : base(fileMetaData.DownloadAddress, async (e) =>
        {
            string NewFilePath = KnownFolders.Downloads.Path + "\\" + fileMetaData.Name;
            int index = 1;
            while (File.Exists(NewFilePath))
            {
                NewFilePath = Path.GetDirectoryName(KnownFolders.Downloads.Path + "\\" + fileMetaData.Name) + "\\" + Path.GetFileNameWithoutExtension(KnownFolders.Downloads.Path + "\\" + fileMetaData.Name) + string.Format("({0})",index) + Path.GetExtension(KnownFolders.Downloads.Path + "\\" + fileMetaData.Name);
                index++;
            }

            File.Move(await e.GetFilePath(), NewFilePath);
        })
        {
            this.fileMetaData = fileMetaData;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
