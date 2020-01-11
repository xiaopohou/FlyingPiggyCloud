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
            ExceptionlessClient.Default.Register();

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!SingleInstanceManager.Check())
            {
                Shutdown();
            }
            else
            {
#warning Message事件处理程序
            }
        }
    }
}
