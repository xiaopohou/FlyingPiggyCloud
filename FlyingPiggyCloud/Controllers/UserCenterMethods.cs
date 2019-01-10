using FlyingPiggyCloud.Controllers.Results;
using FlyingPiggyCloud.Controllers.Results.User;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyingPiggyCloud.Controllers
{
    public class UserCenterMethods : QingzhenyunRequestBase
    {
        public UserCenterMethods(string BaseUri) : base(BaseUri)
        {

        }

        /// <summary>
        /// 发起验证码请求
        /// </summary>
        /// <param name="PhoneNumber">请求验证码的手机号</param>
        /// <returns></returns>
        public async Task<SendRegisterMessageResult> SendingMessageToMobilePhoneNumber(string PhoneNumber)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "phone", PhoneNumber },
            };
            var x = await PostAsync<SendRegisterMessageResult>(JsonConvert.SerializeObject(data), "v1/user/sendRegisterMessage");
            //Token = x.Token;
            return x;
        }

        /// <summary>
        /// 发起注册请求
        /// </summary>
        /// <param name="Name">用户名</param>
        /// <param name="Code">手机验证码</param>
        /// <param name="Password">密码明文，MD5转码在本方法内部进行</param>
        /// <param name="PhoneInfo">该参数来自验证码请求的返回体</param>
        /// <returns></returns>
        public async Task<Results.User.RegisterResponseResult> Register(string Name, string Password, string Code, string PhoneInfo)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "name", Name },
                {"password", Calculators.UserMd5(Password) },
                {"code",Code },
                {"phoneInfo", PhoneInfo }
            };
            var x = await PostAsync<RegisterResponseResult>(JsonConvert.SerializeObject(data), "v1/user/register");
            Token = x.Token;
            return x;
        }

        /// <summary>
        /// 发起密码登录请求
        /// </summary>
        /// <param name="Value">用户手机或者用户名</param>
        /// <param name="Password">用户密码</param>
        /// <returns>登录请求的返回体</returns>
        public async Task<LoginResponseResult> LoginByPassword(string Value, string Password)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "value", Value },
                { "password", Calculators.UserMd5(Password) }
            };
            var x = await PostAsync<LoginResponseResult>(JsonConvert.SerializeObject(data), "v1/user/login");
            Token = x.Token;
            return x;
        }

        /// <summary>
        /// 发起短信验证码请求
        /// </summary>
        /// <param name="PhoneInfo">验证码请求的返回值</param>
        /// <param name="Code">验证码</param>
        /// <returns></returns>
        public async Task<Results.User.LoginResponseResult> LoginByMessageCode(string PhoneInfo, string Code)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "phoneInfo", PhoneInfo },
                { "code", Code }
            };
            var x = await PostAsync<Results.User.LoginResponseResult>(JsonConvert.SerializeObject(data), "v1/user/loginByMessage");
            Token = x.Token;
            return x;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="OldPassword">旧密码</param>
        /// <param name="NewPassword">新密码</param>
        /// <returns></returns>
        public async Task<ResponseMessageBase> ChangePasswordByOldPassword(string OldPassword, string NewPassword)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "OldPassword", Calculators.UserMd5(OldPassword) },
                {"newPassword", Calculators.UserMd5(NewPassword) },
                {"token",Token },
            };
            var x = await PostAsync<ResponseMessageBase>(JsonConvert.SerializeObject(data), "v1/user/changePassword");
            Token = x.Token;
            return x;
        }

        /// <summary>
        /// 发起专用于修改密码的验证码请求
        /// </summary>
        /// <param name="PhoneNumber">请求验证码的手机号</param>
        /// <returns></returns>
        public async Task<SendRegisterMessageResult> SendingMessageToMobilePhoneNumberForChangingPassword(string PhoneNumber)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "phone", PhoneNumber },
            };
            var x = await PostAsync<SendRegisterMessageResult>(JsonConvert.SerializeObject(data), "v1/user/sendChangePasswordMessage2");
            //Token = x.Token;
            return x;
        }

        /// <summary>
        /// 未登录状态下修改密码
        /// </summary>
        /// <param name="PhoneInfo">验证码请求的返回值</param>
        /// <param name="NewPassword">新密码</param>
        /// <param name="Code">验证码</param>
        /// <returns></returns>
        public async Task<ChangePasswordResponesResult> ChangePasswordByMessageCode(string PhoneInfo, string Code, string NewPassword)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "phoneInfo", PhoneInfo },
                {"newPassword", Calculators.UserMd5(NewPassword) },
                {"code",Code },
            };
            var x = await PostAsync<ChangePasswordResponesResult>(JsonConvert.SerializeObject(data), "v1/user/changePasswordByMessage2");
            Token = x.Token;
            return x;
        }
    }
}