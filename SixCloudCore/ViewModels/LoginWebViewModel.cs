using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using SixCloudCore.Controllers;
using SixCloudCore.Models;
using SixCloudCore.Views;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloudCore.ViewModels
{
    internal class LoginWebViewModel : ViewModelBase
    {
        private readonly bool createMainFrame;

        private DestinationInformation DestinationInfo { get; set; }

        private LoginWebView LoginWebView { get; set; }

        private async void InitializeComponent()
        {
            //尝试用已保存的Token获取用户信息
            try
            {
                UserInformation userInfo = await Authentication.GetUserInformation(LocalProperties.Token);

            }
            catch (RequestFailedException)
            {
                DestinationInformation x = await Authentication.CreateDestination();
                DestinationInfo = x;

                LoginUrl = Authentication.GetLoginUrl(DestinationInfo.Destination, out string _);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    LoginWebView = new LoginWebView
                    {
                        DataContext = this
                    };
                    LoginWebView.Show();
                });

                if (!await Authentication.CheckDestination(DestinationInfo))
                {
                    throw new Exception();
                }
            }

            if (createMainFrame)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    new MainFrame().Show();
                    LoginWebView?.Close();
                    Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    new TaskBarButton();
                    Application.Current.Exit += TasksLogger.ExitEventHandler;
                    Application.Current.DispatcherUnhandledException += (sender, e) =>
                    {
                        TasksLogger.ExitEventHandler(sender, e);
                        //System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                        //Application.Current.Shutdown();
                    };
                });
                TasksLogger.StartUpRecovery();
            }
        }

        public string LoginUrl { get; private set; }

        public LoginWebViewModel(bool createMainFrame = true)
        {
            this.createMainFrame = createMainFrame;
            InitializeComponent();
        }
    }
}
