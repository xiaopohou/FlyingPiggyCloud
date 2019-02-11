namespace FlyingPiggyCloud.Controllers
{
    public interface ICompletedTask
    {
        string FileName { get; }

        TaskTypeEnum TaskType { get; }

        string FilePath { get; }

        string Size { get; }

    }

    public enum TaskTypeEnum
    {
        Upload,
        Download
    }
}
