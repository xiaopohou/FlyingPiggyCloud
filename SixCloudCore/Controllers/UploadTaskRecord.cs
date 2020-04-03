//#define Record
//using Exceptionless;

namespace SixCloudCore.Controllers
{
    internal partial class TasksLogger
    {
        private class UploadTaskRecord
        {
            public string LocalFilePath { get; set; }

            public string TargetPath { get; set; }
        }
    }
}