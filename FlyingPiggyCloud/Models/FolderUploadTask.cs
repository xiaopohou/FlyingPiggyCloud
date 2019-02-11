using FlyingPiggyCloud.Controllers;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Wangsu.WcsLib.Core;
using WcsLib.Core;

namespace FlyingPiggyCloud.Models
{
    public class FolderUploadTask : FolderUploadHelper, INotifyPropertyChanged, IUploadTask 
    {
        private static readonly FileSystemMethods fileSystemMethods = new FileSystemMethods(Properties.Settings.Default.BaseUri);

        /// <summary>
        /// 待上传文件的文件名
        /// </summary>
        public string FileName => UploadingDirectory.Name;

        public string Uploaded => UploadedFileCount.ToString();

        public string Total => TotalFileCount.ToString();

        /// <summary>
        /// 上传进度
        /// </summary>
        public double Progress => UploadedFileCount > TotalFileCount ? 100 : (TotalFileCount == 0 ? 0 : UploadedFileCount * 100 / TotalFileCount);

        /// <summary>
        /// 异步开始上传任务
        /// </summary>
        /// <param name="parentUUID">目标目录的UUID</param>
        public async Task StartTask(string parentUUID = null, string parentPath = null)
        {
            await UploadFolder(parentPath,()=>
            {
                OnTaskCompleted(this, new EventArgs());
            });
        }

        private readonly UploadTaskOperator uploadTaskOperator;

        /// <summary>
        /// 取消上传任务，这个指令会在当前块上传结束后执行，如果待上传文件只有一个块则该指令可能无法生效
        /// </summary>
        public void Cancel()
        {
            //uploadTaskOperator.Cancle();
        }

        public string Status { get; set; }

        /// <summary>
        /// 实例化一个上传任务
        /// </summary>
        /// <param name="fullPath">文件的路径</param>
        /// <param name="FileName">文件名</param>
        public FolderUploadTask(DirectoryInfo directoryInfo):base(directoryInfo)
        {
            uploadTaskOperator = new UploadTaskOperator();
            OnNewTaskAdded += (sender,e) => OnPropertyChanged("Total");
            OnSingleFileUploaded += (sender, e) =>
            {
                OnPropertyChanged("Uploaded");
                OnPropertyChanged("Progress");
            };

#if DEBUG
            Status = "新鲜热乎的上传任务";
#endif
        }

        public event TaskStatusChangedEventHandler OnTaskCompleted;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }

}
