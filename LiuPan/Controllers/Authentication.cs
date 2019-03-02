using Newtonsoft.Json;
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
        /// <param name="Name">用户名</param>
        /// <param name="Code">手机验证码</param>
        /// <param name="Password">密码明文，MD5转码在本方法内部进行</param>
        /// <param name="PhoneInfo">该参数来自验证码请求的返回体</param>
        /// <returns></returns>
        public GenericResult<UserInformation> Register(string Name, string Password, string Code, string PhoneInfo)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "name", Name },
#warning 应当修改此处代码以避免PassWord字段明文存储
                {"password", UserMd5(Password) },
                {"code",Code },
                {"phoneInfo", PhoneInfo }
            };
            GenericResult<UserInformation> x = Post<GenericResult<UserInformation>>(JsonConvert.SerializeObject(data), "v1/user/register");
            Token = x.Token;
            return x;
        }

        /// <summary>
        /// 发起密码登录请求
        /// </summary>
        /// <param name="Value">用户手机或者用户名</param>
        /// <param name="Password">用户密码</param>
        /// <returns>登录请求的返回体</returns>
        public GenericResult<UserInformation> LoginByPassword(string Value, string Password)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "value", Value },
#warning 应当修改此处代码以避免PassWord字段明文存储
                { "password", UserMd5(Password) }
            };
            GenericResult<UserInformation> x = Post<GenericResult<UserInformation>>(JsonConvert.SerializeObject(data), "v1/user/login");
            Token = x.Token;
            return x;
        }

        /// <summary>
        /// 发起短信验证码请求
        /// </summary>
        /// <param name="PhoneInfo">验证码请求的返回值</param>
        /// <param name="Code">验证码</param>
        /// <returns></returns>
        public GenericResult<UserInformation> LoginByMessageCode(string PhoneInfo, string Code)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "phoneInfo", PhoneInfo },
                { "code", Code }
            };
            GenericResult<UserInformation> x = Post<GenericResult<UserInformation>>(JsonConvert.SerializeObject(data), "v1/user/loginByMessage");
            Token = x.Token;
            return x;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="OldPassword">旧密码</param>
        /// <param name="NewPassword">新密码</param>
        /// <returns></returns>
        public bool ChangePasswordByOldPassword(string OldPassword, string NewPassword)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "OldPassword", UserMd5(OldPassword) },
                {"newPassword", UserMd5(NewPassword) },
                {"token",Token },
            };
            GenericResult<object> x = Post<GenericResult<object>>(JsonConvert.SerializeObject(data), "v1/user/changePassword");
            Token = x.Token;
            return x.Success;
        }

        /// <summary>
        /// 发起专用于修改密码的验证码请求
        /// </summary>
        /// <param name="PhoneNumber">请求验证码的手机号</param>
        /// <returns></returns>
        public GenericResult<string> SendingMessageToMobilePhoneNumberForChangingPassword(string PhoneNumber)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "phone", PhoneNumber },
            };
            return Post<GenericResult<string>>(JsonConvert.SerializeObject(data), "v1/user/sendChangePasswordMessage2");
        }

        /// <summary>
        /// 未登录状态下修改密码
        /// </summary>
        /// <param name="PhoneInfo">验证码请求的返回值</param>
        /// <param name="NewPassword">新密码</param>
        /// <param name="Code">验证码</param>
        /// <returns></returns>
        public GenericResult<bool> ChangePasswordByMessageCode(string PhoneInfo, string Code, string NewPassword)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "phoneInfo", PhoneInfo },
                {"newPassword", UserMd5(NewPassword) },
                {"code",Code },
            };
            var x = Post<GenericResult<bool>>(JsonConvert.SerializeObject(data), "v1/user/changePasswordByMessage2");
            //Token = x.Token;
            return x;
        }

        private string UserMd5(string input)
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