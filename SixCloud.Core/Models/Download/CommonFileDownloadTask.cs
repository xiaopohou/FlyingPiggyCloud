using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using SixCloudCore.SixTransporter.Downloader;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SixCloud.Core.Models.Download
{
    public class CommonFileDownloadTask : HttpDownloader, ITaskManual
    {
        public event EventHandler TaskComplete;

        public static CommonFileDownloadTask Create(string storagePath, string name, string targetUUID, Guid parent)
        {
            string fullPath = Path.Combine(storagePath, name);
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
            string fullPath = Path.Combine(taskManual.LocalDirectory, taskManual.LocalFileName);
            DownloadTaskInfo taskInfo = File.Exists(fullPath + ".downloading") ? DownloadTaskInfo.Load(fullPath + ".downloading") : new DownloadTaskInfo()
            {
                DownloadUrl = null,
                DownloadPath = fullPath,
                Threads = 4,
            };
            return new CommonFileDownloadTask(taskManual, taskInfo);
        }

        public void Wait()
        {
            if (Status == DownloadStatusEnum.Paused)
            {
                Status = DownloadStatusEnum.Waiting;
            }
        }

        public async void Run()
        {
            if (Status != DownloadStatusEnum.Downloading)
            {
                try
                {
                    FileMetaData downloadDetails = await FileSystem.GetDownloadUrlByIdentity(TargetUUID);
                    string downloadPath = Path.Combine(LocalDirectory, LocalFileName);
                    Info.DownloadUrl = downloadDetails.DownloadAddress;
                    await Task.Run(() => StartDownload());
                }
                catch (RequestFailedException ex) when (ex.Code == "FILE_NOT_FOUND")
                {
                    Status = DownloadStatusEnum.Failed;
                }
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
                    ex.ToSentry().AttachExtraInfo(nameof(CommonFileDownloadTask), this).Submit();
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
                catch (IOException)
                {

                }

                try
                {
                    File.Delete(Path.Combine(LocalDirectory, $"{LocalFileName}.downloading"));
                }
                catch (IOException)
                {

                }
            };

            try
            {
                StopAndSave(true);
            }
            catch (InvalidOperationException)
            {

            }
            finally
            {
                TaskManual.Remove(this);
            }
        }

        public string LocalFileName { get; }

        public string TargetUUID { get; }

        public Guid Guid { get; }

        public Guid Parent { get; }

        public string LocalDirectory { get; }

        public bool IsCompleted { get; private set; } = false;

        protected CommonFileDownloadTask(string storagePath, string name, string targetUUID, Guid parent, DownloadTaskInfo taskInfo) : base(taskInfo)
        {
            LocalFileName = name;
            TargetUUID = targetUUID;
            LocalDirectory = storagePath;
            Guid = Guid.NewGuid();
            Parent = parent;
            DownloadStatusChangedEvent += (sender, e) =>
            {
                IsCompleted = e.NewValue == DownloadStatusEnum.Completed;
                if (IsCompleted)
                {
                    try
                    {
                        File.Delete(Path.Combine(LocalDirectory, $"{LocalFileName}.downloading"));
                    }
                    catch (IOException ex)
                    {
                        ex.Submit();
                    }

                    TaskComplete?.Invoke(this, e);
                }
            };
        }

        protected CommonFileDownloadTask(ITaskManual taskManual, DownloadTaskInfo taskInfo) : this(taskManual.LocalDirectory, taskManual.LocalFileName, taskManual.TargetUUID, taskManual.Parent, taskInfo)
        {
            Guid = taskManual.Guid;
        }

    }

}