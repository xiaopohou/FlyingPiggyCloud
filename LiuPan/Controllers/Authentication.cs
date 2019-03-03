﻿using Newtonsoft.Json;
using SixCloud.Models;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SixCloud.Controllers
{
    internal sealed class Authentication : SixCloudMethordBase
    {
        /// <summary>
        /// 发起验证码请求
        /// </summary>
        /// <param name="phoneNumber">请求验证码的手机号</param>
        /// <returns>PhoneInfo</returns>
        public GenericResult<string> SendingMessageToMobilePhoneNumber(string phoneNumber)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "phone", phoneNumber },
            };
            return Post<GenericResult<string>>(JsonConvert.SerializeObject(data), "v1/user/sendRegisterMessage");
        }

        /// <summary>
        /// 发起注册请求
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="code">手机验证码</param>
        /// <param name="passwordMD5">密码MD5</param>
        /// <param name="phoneInfo">该参数来自验证码请求的返回体</param>
        /// <returns></returns>
        public GenericResult<UserInformation> Register(string name, string passwordMD5, string code, string phoneInfo)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "name", name },
                {"password", passwordMD5 },
                {"code",code },
                {"phoneInfo", phoneInfo }
            };
            GenericResult<UserInformation> x = Post<GenericResult<UserInformation>>(JsonConvert.SerializeObject(data), "v1/user/register");
            if (string.IsNullOrEmpty(x.Token))
            {
                Token = x.Token;
            }
            return x;
        }

        /// <summary>
        /// 发起密码登录请求
        /// </summary>
        /// <param name="value">用户手机或者用户名</param>
        /// <param name="passwordMD5">用户密码MD5</param>
        /// <returns>登录请求的返回体</returns>
        public GenericResult<UserInformation> LoginByPassword(string value, string passwordMD5)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "value", value },
                { "password", passwordMD5 }
            };
            GenericResult<UserInformation> x = Post<GenericResult<UserInformation>>(JsonConvert.SerializeObject(data), "v1/user/login");
            Token = x.Token;
            return x;
        }

        /// <summary>
        /// 发起短信验证码登陆请求
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
            GenericResult<UserInformation> x = Post<GenericResult<UserInformation>>(JsonConvert.SerializeObject(data), "v1/user/loginByMessage");
            Token = x.Token;
            return x;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldPasswordMD5">旧密码</param>
        /// <param name="newPasswordMD5">新密码</param>
        /// <returns></returns>
        public bool ChangePasswordByOldPassword(string oldPasswordMD5, string newPasswordMD5)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "OldPassword", oldPasswordMD5 },
                {"newPassword", newPasswordMD5 },
                {"token",Token },
            };
            GenericResult<object> x = Post<GenericResult<object>>(JsonConvert.SerializeObject(data), "v1/user/changePassword");
            Token = x.Token;
            return x.Success;
        }

        /// <summary>
        /// 发起专用于修改密码的验证码请求
        /// </summary>
        /// <param name="phoneNumber">请求验证码的手机号</param>
        /// <returns></returns>
        public GenericResult<string> SendingMessageToMobilePhoneNumberForChangingPassword(string phoneNumber)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "phone", phoneNumber },
            };
            return Post<GenericResult<string>>(JsonConvert.SerializeObject(data), "v1/user/sendChangePasswordMessage2");
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
                {"newPassword", newPasswordMD5 },
                {"code",code },
            };
            GenericResult<bool> x = Post<GenericResult<bool>>(JsonConvert.SerializeObject(data), "v1/user/changePasswordByMessage2");
            //Token = x.Token;
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
    }
}