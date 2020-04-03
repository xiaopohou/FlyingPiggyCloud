//#define Record
//using Exceptionless;

namespace SixCloudCore.Controllers
{
    internal partial class TasksLogger
    {
        private class DownloadTaskRecord
        {
            public string LocalPath { get; set; }

            public string TargetUUID { get; set; }

            public string Name { get; set; }

            public long BytesReceived { get; set; }

        }
    }
}