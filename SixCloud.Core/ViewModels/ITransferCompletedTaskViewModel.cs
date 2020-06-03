using System;

namespace SixCloud.Core.ViewModels
{
    public interface ITransferCompletedTaskViewModel
    {
        DateTime CompletedTime { get; }
        string Icon { get; }
        string Name { get; }
    }
}