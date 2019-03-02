using SixCloud.Controllers;
using SixCloud.Models;
using SixCloud.Views;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace SixCloud.ViewModels
{
    internal class AuthenticationViewModel : INotifyPropertyChanged
    {
        private readonly Authentication authentication = new Authentication();

        private string PhoneInfo { get; set; }

        private async void SignUp(PasswordBox passwordBox)
        {
            GenericResult<UserInformation> x = await Task.Run(() =>
            {
                return authentication.Register(Username, authentication.UserMd5(passwordBox.Password), VerificationCode, PhoneInfo);
            });
            if (x.Success)
            {
                System.Windows.Window.GetWindow(passwordBox).Close();
                new MainFrame().Show();
            }
        }

        private bool CanSignUp(object paramObj)
        {
            return true;
        }

        private async void SendVerificationCode(object paramObj)
        {
            GenericResult<string> x = await Task.Run(() =>
            {
                return authentication.SendingMessageToMobilePhoneNumber(PhoneNumber);
            });
            if (x.Success)
            {
                PhoneInfo = x.Result;
            }
        }

        private bool CanSendVerificationCode(object paramObj)
        {
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AuthenticationViewModel()
        {
            //SignInCommand = new DependencyCommand<PasswordBox, object>((passwordBox) =>
            //  {

            //  }, (param) =>
            // {
            //     return true;
            // });
        }

        public ICommand SignInCommand { get; private set; }

        public ICommand SignUpCommand { get; private set; }

        public ICommand SendVerificationCodeCommand { get; private set; }

        public bool IsRememberPassword
        {
            get => LocalProperties.IsSavedPassword;
            set => LocalProperties.IsSavedPassword = value;
        }

        public bool IsAutoSignIn
        {
            get => LocalProperties.IsAutoLogin;
            set => LocalProperties.IsAutoLogin = value;
        }

        public string PhoneNumber { get; set; }

        public string Username { get; set; }

        public string VerificationCode { get; set; }

        public bool IsLocked { get; set; }

        public int RemainingTimeBasedSecond { get; set; }

    }

}
