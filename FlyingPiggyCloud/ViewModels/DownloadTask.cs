using FlyingPiggyCloud.Controllers.Results.FileSystem;
using Syroot.Windows.IO;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FlyingPiggyCloud.ViewModels
{
    internal class DownloadTask : FlyingAria2c.DownloadTask, INotifyPropertyChanged
    {
        private readonly FileMetaData fileMetaData;

        public string FileName => fileMetaData.Name;

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
            System.IO.File.Move(await e.GetFilePath(), KnownFolders.Downloads.Path + "\\" + fileMetaData.Name);
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
