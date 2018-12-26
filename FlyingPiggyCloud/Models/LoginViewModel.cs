using System;
using System.ComponentModel;

namespace FlyingPiggyCloud.Models
{
    internal class LoginByPasswordProgressViewModel : Controllers.LoginByPasswordProgress, INotifyPropertyChanged
    {
        /// <summary>
        /// 用户名或手机号
        /// </summary>
        public string UserName
        {
            get => userName;
            set
            {
                userName = value;
                OnPropertyChanged("UserName");
            }
        }
        private string userName;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }
        private string password;

        public bool IsAutoLogin
        {
            get => Controllers.RegistryManager.IsAutoLogin;
            set => Controllers.RegistryManager.IsAutoLogin = value;
        }

        public bool IsSavedPassword
        {
            get => Controllers.RegistryManager.IsSavedPassword;
            set => Controllers.RegistryManager.IsSavedPassword = value;
        }

        public override string FriendlyErrorMessage
        {
            get => base.FriendlyErrorMessage;
            set
            {
                base.FriendlyErrorMessage = value;
                OnPropertyChanged("FriendlyErrorMessage");
            }
        }

        public async void OnLoginButtonClick(object Sender, EventArgs e)
        {
            if (await Login(UserName, Password))
            {
                LoginSuccessful(UserInformation);
            }
            else
            {
            }
        }

        /// <summary>
        /// 成功登陆时触发，并返回登陆的用户信息
        /// </summary>
        public event Action<Controllers.Results.User.UserInformation> LoginSuccessful;

        public LoginByPasswordProgressViewModel() : base(Properties.Settings.Default.BaseUri)
        {
            userName = Controllers.RegistryManager.UserName;
            password = Controllers.RegistryManager.Password;
            Controllers.RegistryManager.CurrentUserInformation = "";
            LoginSuccessful += (userInformation) =>
              {
                  Controllers.RegistryManager.CurrentUserInformation = Newtonsoft.Json.JsonConvert.SerializeObject(userInformation);
              };
        }

        ~LoginByPasswordProgressViewModel()
        {
            Controllers.RegistryManager.UserName = userName;
            if (IsSavedPassword)
            {
                Controllers.RegistryManager.Password = password;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
