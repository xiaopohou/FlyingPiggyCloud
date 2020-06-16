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
            InitializeTaskBarIcon ??= new Action(() => new TaskBarButton());
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
            if (MessageBox.Show("由于傻逼微软在最新版本的win10中移除旧edge相关组件的同时又没有分发新版edge chromium，因此登陆页面无法正确显示，点击确定调用默认浏览器完成登陆。", "一片空白？", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
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
