using SixCloud.Store.Controllers;
using SixCloud.Store.Views;
using System;
using System.Threading.Tasks;
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
            InitializeComponentAsync();
        }
    }
}
