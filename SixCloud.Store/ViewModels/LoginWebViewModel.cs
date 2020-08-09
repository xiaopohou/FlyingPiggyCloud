using SixCloud.Core.Controllers;
using SixCloud.Core.ViewModels;
using SixCloud.Store.Controllers;
using SixCloud.Store.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SixCloud.Store.ViewModels
{
    internal class LoginWebViewModel : Core.ViewModels.LoginWebViewModel
    {
        protected override async Task InitializeComponent()
        {
            CreateLoginWebView ??= new Func<Control>(() => new LoginWebView());
            //InitializeTaskBarIcon ??= new Action(() => new TaskBarButton());
            await base.InitializeComponent();
        }

        private async void InitializeComponentAsync()
        {
            await InitializeComponent();
        }

        public LoginWebViewModel()
        {
            HelpCommand = new DependencyCommand(Help);
            InitializeComponentAsync();
        }

        public DependencyCommand HelpCommand { get; }
        private void Help(object parameter)
        {
            if (MessageBox.Show(Application.Current.FindResource("Lang-DisplayError-Message").ToString(),
                                Application.Current.FindResource("Lang-DisplayError-Title").ToString(),
                                MessageBoxButton.OKCancel,
                                MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                var loginUrl = LoginUrl.Replace("=", "%3D");
                System.Diagnostics.Process.Start("explorer.exe", loginUrl);
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }
}
