using SixCloudCore.FileUploader;

namespace SixCloudCore.ViewModels
{
    internal partial class UploadingFileViewModel
    {
        /// <summary>
        /// 表示一个错误的无法进行的上传任务
        /// </summary>
        private class ErrorTask : IUploadTask
        {
            public string FilePath { get; set; }

            public string Token { get; set; }

            public string Address { get; set; }

            public long CompletedBytes => 0;

            public string Hash => null;

            public long TotalBytes => 999;

            public UploadTaskStatus UploadTaskStatus => UploadTaskStatus.Error;

            public bool TaskOperate(UploadTaskStatus todo)
            {
                return false;
            }
        }
#endif

    }

}
