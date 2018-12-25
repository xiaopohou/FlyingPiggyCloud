using FlyingPiggyCloud.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wangsu.WcsLib.Core;

namespace FlyingPiggyCloud.Models
{
    public class UploadTask:INotifyPropertyChanged
    {
        private static readonly FileSystemMethods fileSystemMethods = new FileSystemMethods(Properties.Settings.Default.BaseUri);

        private readonly string fullPath;

        public string FileName { get; private set; }

        public long UploadedBytes { get; private set; }

        public long TotalBytes { get; private set; }

        public double Progress
        {
            get
            {
                if(TotalBytes!=0)
                {
                    return UploadedBytes * 100 / TotalBytes;
                }
                else
                {
                    return 0D;
                }
            }
        }

        public async void StartTaskAsync(string parentUUID)
        {
            Controllers.Results.ResponesResult<Controllers.Results.FileSystem.UploadResponseResult> x = await fileSystemMethods.UploadFile(FileName, parentUUID, OriginalFilename: FileName);
            UploadProgressHandler uploadProgressHandler = new UploadProgressHandler((a, b) =>
              {
                  App.Current.Dispatcher.Invoke(() =>
                  {
                      UploadedBytes = a;
                      TotalBytes = b;
                      OnPropertyChanged("UploadedBytes");
                      OnPropertyChanged("TotalBytes");
                      OnPropertyChanged("Progress");
                  });
              });
            await Task.Run(() => Upload.Start(x.Result.Token, fullPath, x.Result.UploadUrl, FileName,uploadProgressHandler:uploadProgressHandler));
        }

        public UploadTask(string fullPath, string FileName)
        {
            this.fullPath = fullPath;
            this.FileName = FileName;
            UploadedBytes = 0;
            TotalBytes = 0;
        }

        public event Action OnTaskCompleted;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
