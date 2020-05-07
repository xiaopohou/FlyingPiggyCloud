using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using Sentry;
using SixCloudCore.Controllers;
using SixCloudCore.Models;
using SixCloudCore.Views;
using System;
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
                UserInformation userInfo = await Authentication.GetUserInformation(LocalProperties.Token ?? string.Empty);

            }
            catch (RequestFailedException ex)
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
                    throw new Exception("登陆失败", ex);
                }
            }

            if (createMainFrame)
            {
                OnLoginSuccess();
            }
        }

        private void OnLoginSuccess()
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
                    e.Exception.Submit();
                    //System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    //Application.Current.Shutdown();
                };
            });
            TasksLogger.StartUpRecovery();
        }

        public string LoginUrl { get; private set; }

        public LoginWebViewModel(bool createMainFrame = true)
        {
            this.createMainFrame = createMainFrame;
            InitializeComponent();
        }
    }
}
