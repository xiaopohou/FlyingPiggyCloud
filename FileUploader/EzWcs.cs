using SixTransporter.UploadEngine;
using System;
using System.IO;

namespace SixCloudCore.FileUploader
{
    public static class EzWcs
    {
        private static readonly SliceUploadWorker sliceUploadWorker = new SliceUploadWorker();

        private static readonly SimpleUploadWorker simpleUploadWorker = new SimpleUploadWorker();

        public static IUploadTask NewTask(string filePath, string token, Uri directAddress,Uri partedAddress)
        {
            //var taskInfo = new UploadTaskInfo()
            //{
            //    FilePath = filePath, // 本地文件路径
            //    Threads = 4, // 线程数
            //    Token = token, // 上传Token
            //    UploadUrl = partedAddress.AbsoluteUri // 上传Url
            //};
            //taskInfo.Init(); // 初始化，计算分块信息等.
            ////taskInfo.Save("xx"); // 将分块和进度信息保存到文件
            //var task = new SixUploader(taskInfo);
            //task.UploadStatusChangedEvent += (oldStatus, newStatus, sender) => // 传输状态改变事件
            //{
            //    //Console.WriteLine($"Upload task {sender.Info.FilePath} status changed {oldStatus}->{newStatus}");
            //    //if (newStatus == UploadTaskStatusEnum.Paused)
            //    //    sender.Info.Save("xx"); // 暂停后保存进度
            //};
            ////Console.WriteLine("Upload speed: " + task.Speed); //获取上传速度
            ////Console.WriteLine("Upload progress rate: " + (task.UploadedSize / (float)task.Info.FileSize * 100).ToString("F") + "%"); //进度需要自己计算
            //task.StartUpload();
            //return task;
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Length < SliceUploadWorker.BLOCKSIZE)
            {
                SimpleUploadTask task = new SimpleUploadTask(filePath, token, directAddress);
                simpleUploadWorker.AddTask(task);
                return task;
            }
            else
            {
                SliceUploadTask task = new SliceUploadTask(filePath, token, partedAddress);
                sliceUploadWorker.AddTask(task);
                return task;
            }
        }
    }
}
