using DesktopBridge;
using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using Sentry;
using SixCloud.Core.Controllers;
using SixCloud.Core.Models;
using SixCloud.Core.Views;
using SourceChord.FluentWPF;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using SixCloud.Desktop.Views;
using SixCloud.Desktop.Controllers;

namespace SixCloud.Desktop.ViewModels
{
    internal class LoginWebViewModel : SixCloud.Core.ViewModels.LoginWebViewModel
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
            InitializeComponentAsync();
        }
    }
}
