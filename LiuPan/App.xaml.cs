using Exceptionless;
using SixCloud.Controllers;
using System.Threading;
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
            //TasksLogger.StartUpRecovery();
            new TaskBarButton();
            ExceptionlessClient.Default.Register();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //if(!SingleInstanceManager.Check())
            //{
            //    Shutdown();
            //}
        }
    }
}
