using System.Collections.Generic;

namespace SixCloudCore.Models
{
    internal class DownloadTaskRecord
    {
        public string LocalPath { get; set; }

        public string TargetUUID { get; set; }

        public string Name { get; set; }

    }

    internal class DownloadTaskGroupRecord
    {
        public string LocalPath { get; set; }

        public string TargetUUID { get; set; }

        public string Name { get; set; }

        public IList<DownloadTaskRecord> WaittingList { get; set; }

        public IList<DownloadTaskRecord> RunningList { get; set; }

        public IList<DownloadTaskRecord> CompletedList { get; set; }
    }
}