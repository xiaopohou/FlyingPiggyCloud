using Exceptionless;
using SixCloud.Controllers;
using System.Windows;

namespace SixCloud
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            DownloadTasksLogger.StartUpRecovery();
            TaskBarButton taskBarButton = new TaskBarButton();
            ExceptionlessClient.Default.Register();
        }
    }
}
