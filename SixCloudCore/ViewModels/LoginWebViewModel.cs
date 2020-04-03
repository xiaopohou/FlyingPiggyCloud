using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods.V3;
using SixCloudCore.Views;
using System.Windows;

namespace SixCloudCore.ViewModels
{
    internal class LoginWebViewModel : ViewModelBase
    {
        private DestinationInformation DestinationInfo { get; set; }

        private LoginWebView LoginWebView { get; set; }

        private async void InitializeComponent()
        {
            DestinationInfo = (await Authentication.CreateDestination()).Result;
            LoginUrl = Authentication.GetLoginUrl(DestinationInfo.Destination, out string _);

            Application.Current.Dispatcher.Invoke(() =>
            {
                LoginWebView = new LoginWebView
                {
                    DataContext = this
                };
            });

            if (await Authentication.CheckDestination(DestinationInfo))
            {
                var userInfo = await Authentication.GetUserInformation();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    new MainFrame(userInfo.Result).Show();
                    LoginWebView.Close();
                });
            }
            else
            {

            }

        }

        public string LoginUrl { get; private set; }

        public LoginWebViewModel()
        {
            InitializeComponent();
        }
    }
}
