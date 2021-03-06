﻿using DesktopBridge;
using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using Sentry.Protocol;
using SixCloud.Core.Controllers;
using SourceChord.FluentWPF;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SixCloud.Core.ViewModels
{
    public class LoginWebViewModel : ViewModelBase
    {
        private readonly bool createMainFrame;

        protected static Func<Control> CreateLoginWebView;
        //protected static Action InitializeTaskBarIcon;

        private DestinationInformation DestinationInfo { get; set; }

        private Window LoginWindow { get; set; }

        protected internal virtual async Task InitializeComponent()
        {
            if (!new Helpers().IsRunningAsUwp())
            {
                var newPackageUri = await UpdateHelper.Check();
                if (newPackageUri != default)
                {
                    var newPackageMessageBoxResult = MessageBox.Show(FindLocalizationResource("Lang-UpdateDialog-Message"),
                                                                     FindLocalizationResource("Lang-Update"),
                                                                     MessageBoxButton.OKCancel,
                                                                     MessageBoxImage.Question);
                    if (newPackageMessageBoxResult == MessageBoxResult.OK)
                    {
                        System.Diagnostics.Process.Start("explorer.exe", newPackageUri.ToString());
                        Application.Current.Shutdown();
                    }
                }
            }

            //尝试用已保存的Token获取用户信息
            try
            {
                var userInfo = await Authentication.GetUserInformation(LocalProperties.Token ?? string.Empty);

            }
            catch (RequestFailedException)
            {
                LocalProperties.Token = "";
                var x = await Authentication.CreateDestination();
                DestinationInfo = x;

                LoginUrl = Authentication.GetLoginUrl(DestinationInfo.Destination, out var _);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (Environment.OSVersion.Version >= new Version(6, 2))
                    {
                        LoginWindow = new AcrylicWindow
                        {
                            Title = FindLocalizationResource("Lang-Login"),
                            Height = 650,
                            Width = 400,
                            AcrylicWindowStyle = AcrylicWindowStyle.NoIcon,
                            WindowStartupLocation = WindowStartupLocation.CenterScreen,
                            Content = CreateLoginWebView?.Invoke(),
                            DataContext = this,
                        };
                    }
                    else
                    {
                        LoginWindow = new Window
                        {
                            Title = FindLocalizationResource("Lang-Login"),
                            Height = 650,
                            Width = 400,
                            WindowStyle = WindowStyle.ToolWindow,
                            WindowStartupLocation = WindowStartupLocation.CenterScreen,
                            Content = CreateLoginWebView?.Invoke(),
                            DataContext = this,
                        };
                    }
                    LoginWindow.Show();
                });

                if (!await Authentication.CheckDestination(DestinationInfo))
                {
                    MessageBox.Show(FindLocalizationResource("Lang-LoginTimeOut-Message"), FindLocalizationResource("Lang-TimeOut"), MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown();
                }
            }

            if (createMainFrame)
            {
                await OnLoginSuccess();
            }
        }

        private async Task OnLoginSuccess()
        {
            var currentUser = await Authentication.GetUserInformation();

            SentryAgent.SetUser(new User
            {
                Id = currentUser.UUID.ToString(),
                Username = currentUser.Name,
                Email = currentUser.Email
            }, currentUser);

            await Application.Current.Dispatcher.Invoke(async () =>
            {
                await new MainFrameViewModel().InitializeComponent().ConfigureAwait(true);
                LoginWindow?.Close();

                Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                Application.Current.Exit += (sender, e) => TasksLogger.ExitEventHandler(sender, new Controllers.ExitEventArgs(currentUser));
                Application.Current.DispatcherUnhandledException += (sender, e) =>
                {
                    TasksLogger.ExitEventHandler(sender, new Controllers.ExitEventArgs(currentUser));

                    //尝试自动重启
                    var x = $"{AppDomain.CurrentDomain.BaseDirectory }{AppDomain.CurrentDomain.FriendlyName}.exe";
                    if (File.Exists(x))
                    {
                        //应在此处释放互斥锁
                        System.Diagnostics.Process.Start(x);
                        //Application.Current.Shutdown();
                    }
                };

                new TaskBarButton();
            });
            TasksLogger.StartUpRecovery(currentUser);
        }

        public string LoginUrl { get; private set; }

        public LoginWebViewModel(bool createMainFrame = true)
        {
            this.createMainFrame = createMainFrame;
        }
    }
}
