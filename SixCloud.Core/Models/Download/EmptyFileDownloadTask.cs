﻿using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Utils;
using System;
using System.IO;
using System.Windows;

namespace SixCloud.Core.Models.Download
{
    /// <summary>
    /// 一个大小为0的下载任务
    /// </summary>
    public class EmptyFileDownloadTask : ITaskManual
    {
        public string Completed => Calculators.SizeCalculator(0);

        public double Progress { get; } = 100d;
        public bool Paused = false;

        public string FriendlySpeed => $"{ Calculators.SizeCalculator(0)}/{Application.Current.FindResource("Lang-Units-Second")}";

        public string Total => Calculators.SizeCalculator(0);

        public event EventHandler TaskComplete;

        public void Run()
        {
            if (!Directory.Exists(LocalDirectory))
            {
                Directory.CreateDirectory(LocalDirectory);
            }
            File.Create(Path.Combine(LocalDirectory, LocalFileName)).Close();
            IsCompleted = true;
            TaskComplete?.Invoke(this, EventArgs.Empty);
        }

        public void Stop()
        {
            Paused = true;
        }

        public void Cancel()
        {
            Paused = true;
            try
            {
                File.Delete(Path.Combine(LocalDirectory, LocalFileName));
            }
            catch (IOException ex)
            {
                ex.Submit();
            }
            TaskManual.Remove(this);
        }

        public string TargetUUID { get; }

        public Guid Guid { get; }

        public Guid Parent { get; }

        public string LocalDirectory { get; }

        public string LocalFileName { get; }

        public bool IsCompleted { get; private set; } = false;

        public EmptyFileDownloadTask(string storagePath, string name, string targetUUID, Guid parent)
        {
            LocalFileName = name;
            TargetUUID = targetUUID;
            LocalDirectory = storagePath;
            Guid = Guid.NewGuid();
            Parent = parent;
        }

        public EmptyFileDownloadTask(ITaskManual taskManual) : this(taskManual.LocalDirectory, taskManual.LocalFileName, taskManual.TargetUUID, taskManual.Parent)
        {
            Guid = taskManual.Guid;
        }
    }
}