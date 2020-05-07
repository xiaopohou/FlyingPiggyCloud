using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using Sentry;
using SixCloudCore.Controllers;
using SixCloudCore.Models;
using SixCloudCore.Views;
using System;
using System.IO;
using System.Threading;
using System.Windows;

namespace SixCloudCore.ViewModels
{
    internal class LoginWebViewModel : ViewModelBase
    {
        private readonly bool createMainFrame;

        //private Timer timer;

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
                    //尝试自动重启
                    var x = $"{AppDomain.CurrentDomain.BaseDirectory }SixCloudCore.exe";
                    if (File.Exists(x))
                    {
                        System.Diagnostics.Process.Start(x);
                        Application.Current.Shutdown();
                    }
                };
            });
            TasksLogger.StartUpRecovery();
            //timer = new Timer(async (_) =>
            //{
            //    await Authentication.GetUserInformation();
            //}, null, TimeSpan.FromMinutes(1), TimeSpan.Zero);
        }

        public string LoginUrl { get; private set; }

        public LoginWebViewModel(bool createMainFrame = true)
        {
            this.createMainFrame = createMainFrame;
            InitializeComponent();
        }
    }
}
