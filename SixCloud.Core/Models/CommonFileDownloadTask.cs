using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using QingzhenyunApis.Utils;
using SixCloud.Core.ViewModels;
using SixCloudCore.SixTransporter.Downloader;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SixCloud.Core.Models
{
    public class CommonFileDownloadTask : HttpDownloader, ITaskManual
    {
        public static CommonFileDownloadTask Create(string storagePath, string name, string targetUUID, Guid parent)
        {
            var fullPath = Path.Combine(storagePath, name);
            DownloadTaskInfo taskInfo = File.Exists(fullPath + ".downloading") ? DownloadTaskInfo.Load(fullPath + ".downloading") : new DownloadTaskInfo()
            {
                DownloadUrl = null,
                DownloadPath = fullPath,
                Threads = 4,
            };
            return new CommonFileDownloadTask(storagePath, name, targetUUID, parent, taskInfo);
        }

        public static CommonFileDownloadTask Create(ITaskManual taskManual)
        {
            var fullPath = Path.Combine(taskManual.LocalDirectory, taskManual.LocalFileName);
            DownloadTaskInfo taskInfo = File.Exists(fullPath + ".downloading") ? DownloadTaskInfo.Load(fullPath + ".downloading") : new DownloadTaskInfo()
            {
                DownloadUrl = null,
                DownloadPath = fullPath,
                Threads = 4,
            };
            return new CommonFileDownloadTask(taskManual, taskInfo);
        }

        public async void Run()
        {
            if (Status != DownloadStatusEnum.Downloading)
            {
                FileMetaData downloadDetails = await FileSystem.GetDownloadUrlByIdentity(TargetUUID);
                string downloadPath = Path.Combine(LocalDirectory, LocalFileName);
                Info.DownloadUrl = downloadDetails.DownloadAddress;
                await Task.Run(() => StartDownload());
            }
        }

        public void Stop()
        {
            if (Status == DownloadStatusEnum.Downloading)
            {
                try
                {
                    StopAndSave().Save(Path.Combine(LocalDirectory, $"{LocalFileName}.downloading"));
                }
                catch (NullReferenceException ex)
                {
                    ex.ToSentry().AttachExtraInfo(nameof(DownloadTask), this).Submit();
                }
            }
        }

        public void Cancel()
        {
            AllFileStreamDisposed += (sender, e) =>
            {
                try
                {
                    File.Delete(Path.Combine(LocalDirectory, LocalFileName));
                }
                catch (IOException ex)
                {
                    ex.Submit();
                }

                try
                {
                    File.Delete(Path.Combine(LocalDirectory, $"{LocalFileName}.downloading"));
                }
                catch (IOException ex)
                {
                    ex.Submit();
                }
            };

            StopAndSave(true);
        }

        public string LocalFileName { get; }

        public string TargetUUID { get; }

        public Guid Guid { get; }

        public Guid Parent { get; }

        public string LocalDirectory { get; }

        protected CommonFileDownloadTask(string storagePath, string name, string targetUUID, Guid parent, DownloadTaskInfo taskInfo) : base(taskInfo)
        {
            LocalFileName = name;
            TargetUUID = targetUUID;
            LocalDirectory = storagePath;
            Guid = Guid.NewGuid();
            Parent = parent;
        }

        protected CommonFileDownloadTask(ITaskManual taskManual, DownloadTaskInfo taskInfo) : this(taskManual.LocalDirectory, taskManual.LocalFileName, taskManual.TargetUUID, taskManual.Parent, taskInfo)
        {
            Guid = taskManual.Guid;
        }

    }

}