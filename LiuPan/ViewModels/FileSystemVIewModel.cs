using SixCloud.Controllers;

namespace SixCloud.ViewModels
{
    /// <summary>
    /// 提供File相关接口的调用及VM通知服务
    /// </summary>
    internal abstract class FileSystemViewModel : ViewModelBase
    {
        protected static readonly FileSystem fileSystem = new FileSystem();
    }
}
