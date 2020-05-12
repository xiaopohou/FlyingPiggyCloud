using System.Collections.Generic;

namespace SixCloudCore.Models
{
    internal class DownloadTaskRecord
    {
        public string LocalPath { get; set; }

        public string TargetUUID { get; set; }

        public string Name { get; set; }

    }
}