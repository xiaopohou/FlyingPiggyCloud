namespace FlyingPiggyCloud.Controllers
{
    internal interface ICompletedTask
    {
        string FileName { get; }

        TaskTypeEnum TaskType { get; }

        string FilePath { get; }

        string Size { get; }

    }

    internal enum TaskTypeEnum
    {
        Upload,
        Download
    }
}
