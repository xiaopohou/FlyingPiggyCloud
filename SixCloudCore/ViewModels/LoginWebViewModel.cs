using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods.V3;
using SixCloudCore.Views;
using System.Threading;
using System.Windows;

namespace SixCloudCore.ViewModels
{
    internal class LoginWebViewModel : ViewModelBase
    {
        private DestinationInformation DestinationInfo { get; set; }

        private LoginWebView LoginWebView { get; set; }

        private async void InitializeComponent()
        {
            var x = await Authentication.CreateDestination();
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

            if (await Authentication.CheckDestination(DestinationInfo))
            {
                var userInfo = await Authentication.GetUserInformation();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    new MainFrame(userInfo).Show();
                    LoginWebView.Close();
                });
            }
            else
            {
#warning 失败了应该做点啥
            }
        }

        public string LoginUrl { get; private set; }

        public LoginWebViewModel()
        {
            InitializeComponent();
        }
    }
}
