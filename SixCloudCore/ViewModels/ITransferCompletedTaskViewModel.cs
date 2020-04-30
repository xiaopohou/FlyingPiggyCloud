using System;

namespace SixCloudCore.ViewModels
{
    internal interface ITransferCompletedTaskViewModel
    {
        DateTime CompletedTime { get; }
        string Icon { get; }
        string Name { get; }
    }
}