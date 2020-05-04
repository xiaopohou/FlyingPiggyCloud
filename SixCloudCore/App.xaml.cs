using Sentry;
using SixCloudCore.ViewModels;
using System.Windows;

namespace SixCloudCore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            SentrySdk.Init("https://aa9303eba050450187a9c04653e74be5@o387540.ingest.sentry.io/5222970");
            new LoginWebViewModel();
        }
    }
}
