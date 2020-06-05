using SixCloud.Store.ViewModels;
using System.Windows;

namespace SixCloud.Store
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Core.Core.Initialize();
            new LoginWebViewModel();
        }
    }
}
