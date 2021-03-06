﻿using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using SixCloud.Core.ViewModels;
using SixCloudCore.SixTransporter.Downloader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SixCloud.Core.Models.Download
{
    public class DirectoryDownloadTask : ITaskManual
    {
        public List<ITaskManual> Children { get; } = new List<ITaskManual>();

        public bool Running = true;

        private bool unCalled = true;
        public event EventHandler TaskComplete;

        public DirectoryDownloadTask(string targetUUID, string localDirectory, string localFileName)
        {
            LocalDirectory = localDirectory;
            TargetUUID = targetUUID;
            LocalFileName = localFileName;
            Guid = Guid.NewGuid();
        }

        public DirectoryDownloadTask(ITaskManual taskManual) : this(taskManual.TargetUUID, taskManual.LocalDirectory, taskManual.LocalFileName)
        {
            Guid = taskManual.Guid;
        }


        public string LocalFileName { get; }

        public string TargetUUID { get; }

        public Guid Guid { get; }

        public Guid Parent { get; }

        public string LocalDirectory { get; }

        public int Completed => Children.FindAll(x => x.IsCompleted).Count;

        public int Total => Children.Count;

        public bool Initialized { get; private set; } = false;

        public bool IsCompleted => Initialized && !Children.Any(x => !x.IsCompleted);

        /// <summary>
        /// 初始化任务组
        /// </summary>
        /// <returns></returns>
        public async Task<DirectoryDownloadTask> InitTaskGroup()
        {
            if (!Initialized)
            {
                await DownloadHelper(TargetUUID, Path.Combine(LocalDirectory, LocalFileName), 0);
                Initialized = true;
            }
            return this;

            async Task DownloadHelper(string uuid, string localParentPath, int depthIndex)
            {
                await foreach (var child in FileListViewModel.CreateFileListEnumerator(0, identity: uuid))
                {
                    if (child.Directory)
                    {
                        var nextPath = Path.Combine(localParentPath, child.Name);
                        Directory.CreateDirectory(nextPath);
                        if (depthIndex < 32)
                        {
                            await DownloadHelper(child.UUID, nextPath, depthIndex + 1);
                        }
                        else
                        {
                            ThreadPool.QueueUserWorkItem(async (state) => await DownloadHelper(child.UUID, nextPath, 0), null);
                        }
                    }
                    else
                    {
                        if (!Directory.Exists(localParentPath))
                        {
                            Directory.CreateDirectory(localParentPath);
                        }

                        try
                        {
                            var detail = await FileSystem.GetDetailsByIdentity(child.UUID);
                            ITaskManual newTask;
                            if (detail.Size == 0)
                            {
                                newTask = new EmptyFileDownloadTask(localParentPath, child.Name, child.UUID, Guid);
                            }
                            else
                            {
                                newTask = CommonFileDownloadTask.Create(localParentPath, child.Name, child.UUID, Guid);
                            }

                            newTask.TaskComplete += (sender, e) =>
                            {
                                lock (TaskComplete)
                                {
                                    if (unCalled && IsCompleted)
                                    {
                                        TaskComplete?.Invoke(this, EventArgs.Empty);
                                        unCalled = false;
                                    }
                                }
                            };

                            Children.Add(newTask);
                        }
                        catch (RequestFailedException ex) when (ex.Code == "FILE_NOT_FOUND")
                        {
                            continue;
                        }
                    }
                }
            }
        }

        public void Run()
        {
            Running = true;
            Children.Where(x => x is CommonFileDownloadTask commonFile && commonFile.Status == DownloadStatusEnum.Paused)
                    .ToList()
                    .ForEach(x => (x as CommonFileDownloadTask).Wait());
        }

        public void Stop()
        {
            Running = false;
            Children.ForEach(x => x.Stop());
        }

        public void Cancel()
        {
            Running = false;
            try
            {
                Children.ToList().ForEach(x => x.Cancel());
            }
            finally
            {
                //Directory.Delete(Path.Combine(LocalDirectory, LocalFileName));
            }
        }

        internal void Remove(ITaskManual taskManual)
        {
            Children.Remove(taskManual);

            lock (TaskComplete)
            {
                if (unCalled && IsCompleted)
                {
                    TaskComplete?.Invoke(this, EventArgs.Empty);
                    unCalled = false;
                }
            }

        }

        internal void AddRange(IEnumerable<ITaskManual> enumerable)
        {
            Children.AddRange(enumerable);
            Initialized = true;
        }
    }
}