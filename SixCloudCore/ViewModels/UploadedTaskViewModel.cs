using System;

namespace SixCloudCore.ViewModels
{
    public class UploadedTaskViewModel : ITransferCompletedTaskViewModel
    {
        public string Name { get; set; }

        public DateTime CompletedTime { get; set; }

        public string Icon => throw new NotImplementedException();
    }
}
