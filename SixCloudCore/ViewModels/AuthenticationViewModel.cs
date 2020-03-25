using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods;
using SixCloudCore.Controllers;
using SixCloudCore.Models;
using SixCloudCore.Views;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
//using Exceptionless;
using System.Net.Http;

namespace SixCloudCore.ViewModels
{
    internal class AuthenticationViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly Authentication authentication = new Authentication();
        private string _phoneNumber;
        private string _code;
        private string _verificationCode;

        /// <summary>
        /// 地区/国别码集合
        /// </summary>
        public string[] RegionCollection { get; set; } =
        {
            "(86)中国大陆",
            "(852)香港地区",
            "(853)澳门地区",
            "(886)台湾省",
            "(1)美国",
            "(81)日本",
            "(48)波兰",
        };

        /// <summary>
        /// 申请短信验证码后，同步返回该字段
        /// </summary>
        private string PhoneInfo { get; set; }

        /// <summary>
        /// 登陆成功后被调用
        /// </summary>
        private void OnSignInSuccessful()
        {
            App.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Task.Run(async () => await TasksLogger.StartUpRecovery());
            Application.Current.Exit += TasksLogger.ExitEventHandler;
            new TaskBarButton();
            ThreadPool.QueueUserWorkItem((_) =>
            {
                do
                {
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                    try
                    {
                        authentication.GetUserInformation().Wait();
                    }
                    catch (HttpRequestException)
                    {
#warning 单纯的网络错误无需处理，丢弃
                    }
                    catch (Exception ex)
                    {
                        ex.ToExceptionless().AddTags("手工提交的错误日志").Submit();
                    }
                } while (true);
            });
        }

        private async void SignIn(object param)
        {
            LoadingView loadView = new LoadingView(currentView);
            loadView.FriendlyText.Text = "登录中，请稍后";
            try
            {
                loadView.Show();

                //如果允许自动登录，且保存了上一次的Token，则自动登录
                if (IsAutoSignIn && !string.IsNullOrEmpty(LocalProperties.Token))
                {
                    GenericResult<UserInformation> x = await authentication.GetUserInformation();
                    if (x.Success)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            OnSignInSuccessful();
                            currentView.Close();
                            new MainFrame(x.Result).Show();
                        });
                        return;
                    }
                }

                //如果是验证码登录，且已输入验证码，则验证码登录
                if (IsCodeMode && !string.IsNullOrWhiteSpace(PhoneCode))
                {
                    var x = await authentication.LoginByMessageCode(PhoneInfo, PhoneCode);
                    if (x.Success)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            OnSignInSuccessful();
                            currentView.Close();
                            new MainFrame(x.Result).Show();
                        });
                        return;
                    }
                }

                //如果密码框中输入了信息，则使用密码框中的密码登录
                if (!IsCodeMode && param is PasswordBox passwordBox && !string.IsNullOrEmpty(passwordBox.Password))
                {
                    string passwordMD5 = Authentication.UserMd5(passwordBox.Password);
                    GenericResult<UserInformation> x = await LoginOperate(passwordMD5);
                    if (x.Success)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            OnSignInSuccessful();
                            currentView.Close();
                            new MainFrame(x.Result).Show();
                        });
                        if (IsRememberPassword)
                        {
                            LocalProperties.UserName = PhoneNumber;
                            LocalProperties.CountryCode = Code;
                            LocalProperties.Password = Authentication.UserMd5(passwordBox.Password);
                        }
                        else
                        {
                            LocalProperties.Password = "";
                        }
                    }
                    else
                    {
                        currentView.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(x.Message, "登录失败");
                            currentView.Activate();
                        });
                    }
                    return;
                }

                //如果允许保存密码，且保存了上次登录的密码，且密码框为空，则使用上次保存的密码的md5登录
                if (!IsCodeMode && IsRememberPassword && !string.IsNullOrEmpty(LocalProperties.Password))
                {
                    string passwordMD5 = LocalProperties.Password;
                    GenericResult<UserInformation> x = await LoginOperate(passwordMD5);

                    if (x.Success)
                    {
                        LocalProperties.UserName = PhoneNumber;
                        LocalProperties.CountryCode = Code;
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            OnSignInSuccessful();
                            currentView.Close();
                            new MainFrame(x.Result).Show();
                        });
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(x.Message, "登录失败");
                            currentView.Activate();
                        });
                        LocalProperties.Password = "";
                        OnPropertyChanged(nameof(PasswordBoxHint));
                    }
                    return;

                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("要登录，请输入密码");
                    currentView.Activate();
                });
            }
            finally
            {
                loadView.Close();
            }

            async Task<GenericResult<UserInformation>> LoginOperate(string passwordMD5)
            {
                try
                {
                    return await authentication.LoginByPassword(PhoneNumber, passwordMD5, GetCountryCode());
                }
                catch (LoginUserTooMuchException ex)
                {
                    GenericResult<OnlineDeviceList> getOnlineDeviceList = await authentication.GetOnlineDeviceList();
                    if (getOnlineDeviceList.Success)
                    {
                        string[] devicesSSID = null;
                        LogoutOthersViewModels logoutOthersViewModels = new LogoutOthersViewModels(getOnlineDeviceList);
                        logoutOthersViewModels.ShowDialog();
                        devicesSSID = logoutOthersViewModels.DevicesSSID;

                        GenericResult<bool?> x = await authentication.LogoutOnlineDevices(devicesSSID);

                        if (x.Result == true)
                        {
                            return await LoginOperate(passwordMD5);
                        }
                        else
                        {
                            return ex.Response;
                        }
                    }
                    else
                    {
                        return ex.Response;
                    }
                }
            }
        }

        private bool CanSignIn(object paramObj)
        {
            if (string.IsNullOrEmpty(PhoneNumber))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private async void SignUp(object param)
        {
            if (param is PasswordBox passwordBox)
            {
                GenericResult<bool> x = await authentication.Register(Authentication.UserMd5(passwordBox.Password), VerificationCode, PhoneInfo, GetCountryCode());
                if (x.Success)
                {
                    Window.GetWindow(passwordBox).Close();
                    var loginResult = await authentication.LoginByPassword(PhoneNumber, Authentication.UserMd5(passwordBox.Password), GetCountryCode());
                    new MainFrame(loginResult.Result).Show();
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(x.Message, "注册失败");
                        currentView.Activate();
                    });
                }
            }
        }

        private int GetCountryCode()
        {
            Code = "(86)中国大陆";
            if (!string.IsNullOrWhiteSpace(Code))
            {
                string regularResult = Regex.Match(Code, @"(?<=\()\d+(?=\))").Value;
                if (int.TryParse(regularResult, out int code))
                {
                    return code;
                }
            }
            return 86;
        }

        private bool CanSignUp(object paramObj)
        {
            if (string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(VerificationCode) || string.IsNullOrEmpty(PhoneInfo))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private async void SendVerificationCode(object paramObj)
        {
            GenericResult<string> x;
            if (paramObj as string == "Login")
            {
                x = await Task.Run(() =>
                {
                    return authentication.SendingMessageToMobilePhoneNumberForLogin(PhoneNumber, GetCountryCode());
                });
            }
            else
            {
                x = await Task.Run(() =>
                {
                    return authentication.SendingMessageToMobilePhoneNumber(PhoneNumber, GetCountryCode());
                });
            }
            if (x.Success)
            {
                PhoneInfo = x.Result;
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(x.Message, "发送验证码失败");
                    currentView.Activate();
                });
            }
        }

        private bool CanSendVerificationCode(object paramObj)
        {
            if (string.IsNullOrEmpty(PhoneNumber))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private readonly Window currentView;

        public AuthenticationViewModel(Window viewPointer)
        {
            SignInCommand = new DependencyCommand(SignIn, CanSignIn);
            SignUpCommand = new DependencyCommand(SignUp, CanSignUp);
            SendVerificationCodeCommand = new DependencyCommand(SendVerificationCode, CanSendVerificationCode);
            if (IsRememberPassword && !string.IsNullOrEmpty(LocalProperties.UserName))
            {
                PhoneNumber = LocalProperties.UserName;
                Code = LocalProperties.CountryCode;
                OnPropertyChanged(nameof(PhoneNumber));
                OnPropertyChanged(nameof(Code));
            }
            currentView = viewPointer;
        }

        public Visibility LoginingElement { get; private set; } = Visibility.Collapsed;

        public DependencyCommand SignInCommand { get; private set; }

        public DependencyCommand SignUpCommand { get; private set; }

        public DependencyCommand SendVerificationCodeCommand { get; private set; }

        public bool IsRememberPassword
        {
            get => LocalProperties.IsSavedPassword;
            set
            {
                LocalProperties.IsSavedPassword = value;
                if (!value)
                {
                    LocalProperties.Password = "";
                    OnPropertyChanged(nameof(PasswordBoxHint));
                }
            }
        }

        public string PasswordBoxHint
        {
            get
            {
                if (IsRememberPassword && !string.IsNullOrEmpty(LocalProperties.Password))
                {
                    return "[要更改已保存的密码，请点击这里]";
                }
                else
                {
                    return "密码";
                }
            }
        }

        public bool IsAutoSignIn
        {
            get => LocalProperties.IsAutoLogin;
            set => LocalProperties.IsAutoLogin = value;
        }

        public string PhoneNumber { get => _phoneNumber; set { _phoneNumber = value; SendVerificationCodeCommand.OnCanExecutedChanged(this, null); SignUpCommand.OnCanExecutedChanged(this, null); SignInCommand.OnCanExecutedChanged(this, null); OnPropertyChanged(nameof(PhoneNumber)); } }

        public string Code
        {
            get => _code; set { _code = value; SignUpCommand.OnCanExecutedChanged(this, null); }
        }

        public string VerificationCode { get => _verificationCode; set { _verificationCode = value; SignUpCommand.OnCanExecutedChanged(this, null); } }

        public bool IsLocked { get; set; }

        public int RemainingTimeBasedSecond { get; set; }

        public bool IsCodeMode { get; set; } = false;

        public string PhoneCode { get; set; }
    }

}
