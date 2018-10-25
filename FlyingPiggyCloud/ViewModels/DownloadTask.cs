using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyingAria2c;
using FlyingPiggyCloud.Controllers.Results.FileSystem;

namespace FlyingPiggyCloud.ViewModels
{
    internal class DownloadTask : FlyingAria2c.DownloadTask, INotifyPropertyChanged
    {
        private readonly FileMetaData fileMetaData;

        public string FileName { get; set; }

        public new async Task RefreshStatus()
        {
            await base.RefreshStatus();
            OnPropertyChanged("Status");
            OnPropertyChanged("Progress");
            OnPropertyChanged("DownloadSpeed");
        }

        public DownloadTask(FileMetaData fileMetaData, Action<FlyingAria2c.DownloadTask> CompletedEventHandle = null) : base(fileMetaData.DownloadAddress, CompletedEventHandle)
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
