using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace QingzhenyunApis.QingzhenyunMethods
{
    internal sealed class Authentication : SixCloudMethordBase
    {
        /// <summary>
        /// 发起注册验证码请求
        /// </summary>
        /// <param name="phoneNumber">请求验证码的手机号</param>
        /// <returns>PhoneInfo</returns>
        public GenericResult<string> SendingMessageToMobilePhoneNumber(string phoneNumber, int countryCode)
        {
            var data = new { phone = phoneNumber, countryCode };
            return Post<GenericResult<string>>(JsonConvert.SerializeObject(data), "v2/user/sendRegisterMessage", new Dictionary<string, string>(), out _);
        }

        /// <summary>
        /// 发起登录验证码请求
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public GenericResult<string> SendingMessageToMobilePhoneNumberForLogin(string phoneNumber, int countryCode)
        {
            var data = new { phone = phoneNumber, countryCode };
            return Post<GenericResult<string>>(JsonConvert.SerializeObject(data), "v2/user/sendLoginMessage", new Dictionary<string, string>(), out _);
        }

        /// <summary>
        /// 发起注册请求
        /// </summary>
        /// <param name="code">手机验证码</param>
        /// <param name="passwordMD5">密码MD5</param>
        /// <param name="phoneInfo">该参数来自验证码请求的返回体</param>
        /// <returns></returns>
        public GenericResult<bool> Register(string passwordMD5, string code, string phoneInfo, int countryCode)
        {
            var data = new { password = passwordMD5, code, phoneInfo, countryCode };
            GenericResult<bool> x = Post<GenericResult<bool>>(JsonConvert.SerializeObject(data), "v2/user/register", new Dictionary<string, string>(), out _);
            return x;
        }

        /// <summary>
        /// 发起密码登录请求
        /// </summary>
        /// <param name="value">用户手机或者用户名</param>
        /// <param name="passwordMD5">用户密码MD5</param>
        /// <returns>登录请求的返回体</returns>
        public GenericResult<UserInformation> LoginByPassword(string value, string passwordMD5, int countryCode)
        {
            var data = new { value, password = passwordMD5, countryCode };
            GenericResult<UserInformation> x = Post<GenericResult<UserInformation>>(JsonConvert.SerializeObject(data), "v2/user/login", new Dictionary<string, string>(), out WebHeaderCollection webHeaderCollection);
            if (!x.Success && x.Code == "LOGIN_USER_TOO_MUCH")
            {
                Token = null;
                throw new LoginUserTooMuchException(webHeaderCollection.Get("qingzhen-token"), x.Message, x);
            }
            Token = webHeaderCollection.Get("qingzhen-token");
            return x;
        }

        /// <summary>
        /// 发起短信验证码登录请求
        /// </summary>
        /// <param name="phoneInfo">验证码请求的返回值</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        public GenericResult<UserInformation> LoginByMessageCode(string phoneInfo, string code)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "phoneInfo", phoneInfo },
                { "code", code }
            };
            GenericResult<UserInformation> x = Post<GenericResult<UserInformation>>(JsonConvert.SerializeObject(data), "v2/user/loginWithMessage", new Dictionary<string, string>(), out WebHeaderCollection webHeaderCollection);
            if (!x.Success && x.Code == "LOGIN_USER_TOO_MUCH")
            {
                Token = null;
                throw new LoginUserTooMuchException(webHeaderCollection.Get("qingzhen-token"), x.Message, x);
            }
            Token = webHeaderCollection.Get("qingzhen-token");
            return x;
        }

        /// <summary>
        /// 获取当前登录的其他设备列表
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public GenericResult<OnlineDeviceList> GetOnlineDeviceList(string token, out string nextToken)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            GenericResult<OnlineDeviceList> x = Post<GenericResult<OnlineDeviceList>>(JsonConvert.SerializeObject(data), "v2/user/online", new Dictionary<string, string>
            {
                { "Qingzhen-Token",token }
            }, out WebHeaderCollection webHeaderCollection);
            nextToken = webHeaderCollection.Get("qingzhen-token");
            return x;
        }

        /// <summary>
        /// 根据ssid踢掉设备
        /// </summary>
        /// <param name="ssidArray"></param>
        /// <returns></returns>
        public GenericResult<bool?> LogoutOnlineDevices(string token, string[] ssidArray)
        {
            Dictionary<string, string[]> data = new Dictionary<string, string[]>
            {
                {"ssid",ssidArray }
            };
            GenericResult<bool?> x = Post<GenericResult<bool?>>(JsonConvert.SerializeObject(data), "v2/user/logoutOther", new Dictionary<string, string>
            {
                { "Qingzhen-Token",token }
            }, out _);
            return x;
        }

        /// <summary>
        /// 获取用户信息（用于刷新Token）
        /// </summary>
        /// <param name="ssidArray"></param>
        /// <returns></returns>
        public GenericResult<UserInformation> GetUserInformation()
        {
            Dictionary<string, string[]> data = new Dictionary<string, string[]>();
            GenericResult<UserInformation> x = Post<GenericResult<UserInformation>>(JsonConvert.SerializeObject(data), "v2/user/info", new Dictionary<string, string>
            {
                { "Qingzhen-Token",Token }
            }, out WebHeaderCollection webHeaderCollection);
            Token = webHeaderCollection.Get("qingzhen-token");
            return x;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldPasswordMD5">旧密码</param>
        /// <param name="newPasswordMD5">新密码</param>
        /// <returns></returns>
        public GenericResult<object> ChangePasswordByOldPassword(string oldPasswordMD5, string newPasswordMD5)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "OldPassword", oldPasswordMD5 },
                {"newPassword", newPasswordMD5 },
            };
            while (string.IsNullOrWhiteSpace(Token))
            {
                LoginView GetToken = new LoginView();
                GetToken.ShowDialog();
            }
            GenericResult<object> x = Post<GenericResult<object>>(JsonConvert.SerializeObject(data), "v2/user/changePassword", new Dictionary<string, string>
            {
                { "Qingzhen-Token",Token }
            }, out WebHeaderCollection webHeaderCollection);
            Token = webHeaderCollection.Get("qingzhen-token");
            return x;
        }

        /// <summary>
        /// 发起专用于修改密码的验证码请求
        /// </summary>
        /// <param name="phoneNumber">请求验证码的手机号</param>
        /// <returns></returns>
        public GenericResult<string> SendingMessageToMobilePhoneNumberForChangingPassword(string phoneNumber, int countryCode)
        {
            var data = new { phone = phoneNumber, countryCode };
            return Post<GenericResult<string>>(JsonConvert.SerializeObject(data), "v2/user/sendChangePasswordMessage", new Dictionary<string, string>(), out _);
        }

        /// <summary>
        /// 未登录状态下修改密码
        /// </summary>
        /// <param name="phoneInfo">验证码请求的返回值</param>
        /// <param name="newPasswordMD5">新密码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        public GenericResult<bool> ChangePasswordByMessageCode(string phoneInfo, string code, string newPasswordMD5)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "phoneInfo", phoneInfo },
                {"password", newPasswordMD5 },
                {"code",code },
            };
            GenericResult<bool> x = Post<GenericResult<bool>>(JsonConvert.SerializeObject(data), "v2/user/changePasswordWithMessage", new Dictionary<string, string>(), out _);
            //Token = x.Token;
            return x;
        }

        /// <summary>
        /// 登录后修改用户名
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public GenericResult<bool> ChangeUserName(string newName)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "name", newName },
            };
            while (string.IsNullOrWhiteSpace(Token))
            {
                LoginView GetToken = new LoginView();
                GetToken.ShowDialog();
            }
            GenericResult<bool> x = Post<GenericResult<bool>>(JsonConvert.SerializeObject(data), "v2/user/changeName", new Dictionary<string, string>
            {
                { "Qingzhen-Token",Token }
            }, out WebHeaderCollection webHeaderCollection);
            Token = webHeaderCollection.Get("qingzhen-token");
            return x;
        }

        /// <summary>
        /// 用户输入的密码通过此方法转换为MD5值
        /// </summary>
        /// <param name="input">用户输入</param>
        /// <returns></returns>
        public string UserMd5(string input)
        {
            byte[] bytes = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < bytes.Length; i++)
            {
                sBuilder.Append(bytes[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        [Serializable]
        public class LoginUserTooMuchException : Exception
        {
            public LoginUserTooMuchException(string token, string message, GenericResult<UserInformation> response) : base(message)
            {
                Token = token;
                Response = response;
            }

            public string Token { get; set; }

            public GenericResult<UserInformation> Response { get; set; }

            protected LoginUserTooMuchException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            {
                throw new NotImplementedException();
            }

            public LoginUserTooMuchException()
            {
            }

            public LoginUserTooMuchException(string message) : base(message)
            {
            }

            public LoginUserTooMuchException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }
    }
}