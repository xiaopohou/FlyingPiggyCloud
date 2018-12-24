using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;

namespace FlyingPiggyCloud.Models
{
    class RegisterViewModel:Controllers.RegisterProgress, INotifyPropertyChanged
    {
        /// <summary>
        /// 重写的FriendlyErrorMessage Set访问器以实现INotifyPropertyChanged接口
        /// </summary>
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
        /// 用户名
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        private string name;

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                OnPropertyChanged("PassWord");
            }
        }
        private string password;

        /// <summary>
        /// 手机验证码
        /// </summary>
        public string Code
        {
            get
            {
                return code;
            }
            set
            {
                code = value;
                OnPropertyChanged("Code");
            }
        }
        private string code;

        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumber
        {
            get
            {
                return phoneNumber;
            }
            set
            {
                phoneNumber = value;
                OnPropertyChanged("PhoneNumber");
            }
        }
        private string phoneNumber;

        /// <summary>
        /// 指示是否显示FriendlyErrorMessage
        /// </summary>
        public bool IsEnableErrorMessage
        {
            get
            {
                return isEnableErrorMessage;
            }
            set
            {
                isEnableErrorMessage = value;
                OnPropertyChanged("IsEnableErrorMessage");
            }
        }
        private bool isEnableErrorMessage = false;

        /// <summary>
        /// 指示注册按钮是否可用
        /// </summary>
        public bool IsEnableRegisterButton
        {
            get
            {
                return isEnableRegisterButton;
            }
            set
            {
                isEnableRegisterButton = value;
                OnPropertyChanged("IsEnableRegisterButton");
            }
        }
        private bool isEnableRegisterButton = false;

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
        /// 验证码按钮Click事件处理程序
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        public async void OnSendMessageButtonClick(object Sender, EventArgs e)
        {
            FriendlyErrorMessage = "";
            if (!Regex.IsMatch(PhoneNumber, "^[1]+[3,4,5,7,8,9]+\\d{9}$"))
                FriendlyErrorMessage = "手机号码格式不正确";
            else
            {
                IsEnableSendMessageButton = false;
                await GetCodeMessage(PhoneNumber);
            }
        }

        /// <summary>
        /// 注册按钮Click事件处理程序
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        public async void OnRegisterButtonClick(object Sender, EventArgs e)
        {
            //暂时在这里实现表单项的验证逻辑
            if (string.IsNullOrEmpty(Name))
                FriendlyErrorMessage = "用户名不得为空";
            else if (Name.Length > 8)
                FriendlyErrorMessage = "用户名过长";
            else if (Regex.IsMatch(Name, @"\W"))
                FriendlyErrorMessage = "用户名不应包含特殊字符";
            else if (PassWord.Length>32||PassWord.Length<8)
                FriendlyErrorMessage = "合法的密码长度8-32位";
            else if (!Regex.IsMatch(PassWord, @"\W") && PassWord.Length < 10)
                FriendlyErrorMessage = "如果您的密码中不包含特殊字符，请确保长度至少为10位";
            else if (await Register(Name, PassWord, Code))
            {
                RegisterSuccessful(UserInformation);
            }
            else
            {

            }
        }

        public event Action<Controllers.Results.User.UserInformation> RegisterSuccessful;

        public RegisterViewModel() : base(Properties.Settings.Default.BaseUri)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
