using System;
using System.Collections.Generic;
using System.Reflection;

namespace SixCloud.Core.Models
{
    public class DownloadTaskGroupRecord
    {
        public string LocalPath { get; set; }

        public string TargetUUID { get; set; }

        public string Name { get; set; }

        [Obsolete]
        public IList<DownloadTaskRecord> WaittingList { get; set; }

        [Obsolete]
        public IList<DownloadTaskRecord> RunningList { get; set; }

        public IList<DownloadTaskRecord> CompletedList { get; set; }

        public IList<IDownloadTask> TaskList { get; set; }
    }
}