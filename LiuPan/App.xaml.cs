using SixCloud.Controllers;
using System.Windows;
using static SixCloud.Controllers.TransmissionProgressController;

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
            DownloadingCache.StartUpRecovery();
            TaskBarButton taskBarButton = new TaskBarButton();
        }
    }
}
