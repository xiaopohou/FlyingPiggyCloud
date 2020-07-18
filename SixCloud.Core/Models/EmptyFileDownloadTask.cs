using QingzhenyunApis.Utils;
using System;
using System.IO;
using System.Linq;

namespace SixCloud.Core.Models
{
    /// <summary>
    /// 一个大小为0的下载任务
    /// </summary>
    public class EmptyFileDownloadTask : ITaskManual
    {
        public string Completed => Calculators.SizeCalculator(0);

        public double Progress { get; } = 100d;

        public string FriendlySpeed => Calculators.SizeCalculator(0) + "/秒";

        public string Total => Calculators.SizeCalculator(0);

        public void Run()
        {
            if (!Directory.Exists(LocalDirectory))
            {
                Directory.CreateDirectory(LocalDirectory);
            }
            File.Create(Path.Combine(LocalDirectory, LocalFileName)).Close();
            DownloadCompleted?.Invoke(this, null);
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public event EventHandler DownloadCompleted;

        public string TargetUUID { get; }

        public Guid Guid { get; }

        public Guid Parent { get; }

        public string LocalDirectory { get; }

        public string LocalFileName { get; }

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