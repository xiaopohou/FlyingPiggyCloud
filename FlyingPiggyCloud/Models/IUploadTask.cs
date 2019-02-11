using FlyingPiggyCloud.Controllers;
using System;
using System.Threading.Tasks;

namespace FlyingPiggyCloud.Models
{
    public delegate void TaskStatusChangedEventHandler(object sender, EventArgs e);

    public interface IUploadTask
    {
        string FileName { get; }

        double Progress { get; }

        string Uploaded { get; }

        string Total { get; }

        void Cancel();

        Task StartTask(string parentUUID=null, string parentPath=null);

        string Status { get; }

        event TaskStatusChangedEventHandler OnTaskCompleted;
    }
}
