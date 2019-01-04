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

        /// <summary>
        /// 待上传文件的绝对路径
        /// </summary>
        private readonly string fullPath;

        /// <summary>
        /// 待上传文件的文件名
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 已上传的字节数
        /// </summary>
        public long UploadedBytes { get; private set; }

        /// <summary>
        /// 文件的总字节数
        /// </summary>
        public long TotalBytes { get; private set; }


        /// <summary>
        /// 上传进度
        /// </summary>
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

        /// <summary>
        /// 异步开始上传任务
        /// </summary>
        /// <param name="parentUUID">目标目录的UUID</param>
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
            OnTaskCompleted?.Invoke(this,new EventArgs());
        }

        /// <summary>
        /// 实例化一个上传任务
        /// </summary>
        /// <param name="fullPath">文件的路径</param>
        /// <param name="FileName">文件名</param>
        public UploadTask(string fullPath, string FileName)
        {
            this.fullPath = fullPath;
            this.FileName = FileName;
            UploadedBytes = 0;
            TotalBytes = 0;
        }

        public delegate void TaskStatuChanged(object sender, EventArgs e);
        public event TaskStatuChanged OnTaskCompleted;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
