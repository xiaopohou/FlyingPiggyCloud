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
using System.Windows.Threading;

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
            var newPackageUri = await UpdateHelper.Check();
            if (newPackageUri != default)
            {
                var newPackageMessageBoxResult = MessageBox.Show("发现新的软件包，点击确定下载或者点击取消继续使用当前版本", "更新", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (newPackageMessageBoxResult == MessageBoxResult.OK)
                {
                    System.Diagnostics.Process.Start("explorer.exe", newPackageUri.ToString());
                    Application.Current.Shutdown();
                }
            }

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

        private async void OnLoginSuccess()
        {
            var currentUser = await Authentication.GetUserInformation();
            Application.Current.Dispatcher.Invoke(() =>
            {
                new MainFrame().Show();
                LoginWebView?.Close();

                Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                new TaskBarButton();
                Application.Current.Exit += (sender, e) => TasksLogger.ExitEventHandler(sender, new Controllers.ExitEventArgs(currentUser));
                Application.Current.DispatcherUnhandledException += (sender, e) =>
                {
                    TasksLogger.ExitEventHandler(sender, new Controllers.ExitEventArgs(currentUser));
                    e.Exception.ToSentry().TreatedBy(nameof(DispatcherUnhandledExceptionEventHandler)).Submit();

                    //尝试自动重启
                    var x = $"{AppDomain.CurrentDomain.BaseDirectory }SixCloudCore.exe";
                    if (File.Exists(x))
                    {
                        //应在此处释放互斥锁
                        System.Diagnostics.Process.Start(x);
                        //Application.Current.Shutdown();
                    }
                };
            });
            TasksLogger.StartUpRecovery(currentUser);
        }

        public string LoginUrl { get; private set; }

        public LoginWebViewModel(bool createMainFrame = true)
        {
            this.createMainFrame = createMainFrame;
            InitializeComponent();
        }
    }
}
