using SixCloud.Core.Controllers;

using SixCloud.Desktop.Views;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SixCloud.Desktop.ViewModels
{
    internal class LoginWebViewModel : SixCloud.Core.ViewModels.LoginWebViewModel
    {
        protected override async Task InitializeComponent()
        {
            CreateLoginWebView ??= new Func<Control>(() => new LoginWebView());
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
