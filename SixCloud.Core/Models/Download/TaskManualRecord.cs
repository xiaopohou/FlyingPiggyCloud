using System;

namespace SixCloud.Core.Models.Download
{
    public class TaskManualRecord : ITaskManual
    {
        public string TargetUUID { get; set; }

        public Guid Guid { get; set; }

        public Guid Parent { get; set; }

        public string LocalFileName { get; set; }

        public string LocalDirectory { get; set; }

        public bool IsCompleted { get; set; }

        public event EventHandler TaskComplete;

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}