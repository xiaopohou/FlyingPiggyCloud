using SixCloud.Controllers;
using SixCloud.Models;
using System.ComponentModel;

namespace SixCloud.ViewModels
{
    /// <summary>
    /// 提供File相关接口的调用及VM通知服务
    /// </summary>
    internal abstract class FileSystemViewModel : INotifyPropertyChanged
    {
        protected static readonly FileSystem fileSystem = new FileSystem();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
