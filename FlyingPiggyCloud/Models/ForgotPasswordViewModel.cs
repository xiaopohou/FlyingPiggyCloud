using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlyingPiggyCloud.Models
{
    internal class ForgotPasswordViewModel : Controllers.ForgotPasswordProgress, INotifyPropertyChanged
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumber
        {
            get => phoneNumber;
            set
            {
                phoneNumber = value;
                OnPropertyChanged("PhoneNumber");
            }
        }
        private string phoneNumber;

        /// <summary>
        /// 指示验证码按钮是否可用
        /// </summary>
        public bool IsEnableSendMessageButton
        {
            get
            {
                return isEnableSendMessageButton;
            }
            set
            {
                isEnableSendMessageButton = value;
                OnPropertyChanged("IsEnableSendMessageButton");
            }
        }
        private bool isEnableSendMessageButton = true;

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

        /// <summary>
        /// 重复密码
        /// </summary>
        public string PasswordAgain
        {
            get => passwordAgain;
            set
            {
                passwordAgain = value;
                OnPropertyChanged("PasswordAgain");
            }
        }
        private string passwordAgain;

        /// <summary>
        /// 验证码
        /// </summary>
        public string Code
        {
            get => code;
            set
            {
                code = value;
                OnPropertyChanged("Code");
            }
        }
        private string code;

        public override string FriendlyErrorMessage
        {
            get => base.FriendlyErrorMessage;
            set
            {
                base.FriendlyErrorMessage = value;
                OnPropertyChanged("FriendlyErrorMessage");
            }
        }

        /// <summary>
        /// 验证码按钮Click事件处理程序
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        public async void OnSendMessageButtonClick(object Sender, EventArgs e)
        {
            FriendlyErrorMessage = "";
            if (Regex.IsMatch(PhoneNumber, "^[1]+[3,4,5,7,8,9]+\\d{9}$"))
            {
                IsEnableSendMessageButton = false;
                await SendMessageCode(PhoneNumber);
            }
            else
            {
                FriendlyErrorMessage = "手机号码格式不正确";
            }
        }

        /// <summary>
        /// 修改密码按钮点击事件处理程序
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        public async void OnChangePasswordButtonClick(object Sender, EventArgs e)
        {
            if (await ChangePassword(Password, Code))
            {
                ChangingSuccessful();
            }
            else
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// 密码修改成功时触发
        /// </summary>
        public event Action ChangingSuccessful;

        public ForgotPasswordViewModel() : base(Properties.Settings.Default.BaseUri)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
