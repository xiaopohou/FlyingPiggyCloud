using System;

namespace SixCloud.Core.ViewModels
{
    public class UploadedTaskViewModel : ITransferCompletedTaskViewModel
    {
        public string Name { get; set; }

        public DateTime CompletedTime { get; set; }

        public string Icon => "\uf382";
    }
}
