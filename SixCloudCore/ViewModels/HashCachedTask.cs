﻿using SixCloudCore.FileUploader;

namespace SixCloudCore.ViewModels
{
    internal partial class UploadingFileViewModel
    {
        /// <summary>
        /// 一个可以秒传的上传任务
        /// </summary>
        private class HashCachedTask : IUploadTask
        {
            public string FilePath { get; set; }

            public HashCachedTask()
            {
                CompletedBytes = 999;
                TotalBytes = 999;
            }

            public string Token { get; set; }

            public string Address { get; set; }

            public long CompletedBytes { get; set; }

            public string Hash { get; set; }

            public long TotalBytes { get; set; }

            public UploadTaskStatus UploadTaskStatus => UploadTaskStatus.Completed;

            public bool TaskOperate(UploadTaskStatus todo)
            {
                return false;
            }
        }
#endif

    }

}
