using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods.V3
{
    public sealed class Authentication : SixCloudMethordBase
    {
        ///// <summary>
        ///// 发起注册验证码请求
        ///// </summary>
        ///// <param name="phoneNumber">请求验证码的手机号</param>
        ///// <returns>PhoneInfo</returns>
        //public async Task<GenericResult<string>> SendingMessageToMobilePhoneNumber(string phoneNumber, int countryCode)
        //{
        //    var data = new { phone = phoneNumber, countryCode };
        //    return await PostAsync<GenericResult<string>>(JsonConvert.SerializeObject(data), "/v2/user/sendRegisterMessage");
        //}

        ///// <summary>
        ///// 发起登录验证码请求
        ///// </summary>
        ///// <param name="phoneNumber"></param>
        ///// <returns></returns>
        //public async Task<GenericResult<string>> SendingMessageToMobilePhoneNumberForLogin(string phoneNumber, int countryCode)
        //{
        //    var data = new { phone = phoneNumber, countryCode };
        //    return await PostAsync<GenericResult<string>>(JsonConvert.SerializeObject(data), "/v2/user/sendLoginMessage");
        //}

        ///// <summary>
        ///// 发起注册请求
        ///// </summary>
        ///// <param name="code">手机验证码</param>
        ///// <param name="passwordMD5">密码MD5</param>
        ///// <param name="phoneInfo">该参数来自验证码请求的返回体</param>
        ///// <returns></returns>
        //public async Task<GenericResult<bool>> Register(string passwordMD5, string code, string phoneInfo, int countryCode)
        //{
        //    var data = new { password = passwordMD5, code, phoneInfo, countryCode };
        //    return await PostAsync<GenericResult<bool>>(JsonConvert.SerializeObject(data), "/v2/user/register");
        //}

        ///// <summary>
        ///// 发起密码登录请求
        ///// </summary>
        ///// <param name="value">用户手机或者用户名</param>
        ///// <param name="passwordMD5">用户密码MD5</param>
        ///// <returns>登录请求的返回体</returns>
        //public async Task<GenericResult<UserInformation>> LoginByPassword(string value, string passwordMD5, int countryCode)
        //{
        //    var data = new { value, password = passwordMD5, countryCode };
        //    GenericResult<UserInformation> x = await PostAsync<GenericResult<UserInformation>>(JsonConvert.SerializeObject(data), "/v2/user/login");

        //    if (!x.Success && x.Code == "LOGIN_USER_TOO_MUCH")
        //    {
        //        throw new LoginUserTooMuchException(Token, x.Message, x);
        //    }

        //    return x;
        //}

        ///// <summary>
        ///// 发起短信验证码登录请求
        ///// </summary>
        ///// <param name="phoneInfo">验证码请求的返回值</param>
        ///// <param name="code">验证码</param>
        ///// <returns></returns>
        //public async Task<GenericResult<UserInformation>> LoginByMessageCode(string phoneInfo, string code)
        //{
        //    var data = new { phoneInfo, code };
        //    GenericResult<UserInformation> x = await PostAsync<GenericResult<UserInformation>>(JsonConvert.SerializeObject(data), "/v2/user/loginWithMessage");
        //    if (!x.Success && x.Code == "LOGIN_USER_TOO_MUCH")
        //    {
        //        throw new LoginUserTooMuchException(Token, x.Message, x);
        //    }
        //    return x;
        //}

        ///// <summary>
        ///// 获取当前登录的其他设备列表
        ///// </summary>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //public async Task<GenericResult<OnlineDeviceList>> GetOnlineDeviceList()
        //{
        //    var data = new { };
        //    return await PostAsync<GenericResult<OnlineDeviceList>>(JsonConvert.SerializeObject(data), "/v2/user/online", false);
        //}

        ///// <summary>
        ///// 根据ssid踢掉设备
        ///// </summary>
        ///// <param name="ssidArray"></param>
        ///// <returns></returns>
        //public async Task<GenericResult<bool?>> LogoutOnlineDevices(string[] ssidArray)
        //{
        //    var data = new { ssid = ssidArray };
        //    return await PostAsync<GenericResult<bool?>>(JsonConvert.SerializeObject(data), "/v2/user/logoutOther", false);
        //}

        ///// <summary>
        ///// 获取用户信息（用于刷新Token）
        ///// </summary>
        ///// <param name="ssidArray"></param>
        ///// <returns></returns>
        //public async Task<GenericResult<UserInformation>> GetUserInformation()
        //{
        //    var data = new { };
        //    return await PostAsync<GenericResult<UserInformation>>(JsonConvert.SerializeObject(data), "/v2/user/info", false);
        //}

        ///// <summary>
        ///// 修改密码
        ///// </summary>
        ///// <param name="oldPasswordMD5">旧密码</param>
        ///// <param name="newPasswordMD5">新密码</param>
        ///// <returns></returns>
        //public async Task<GenericResult<object>> ChangePasswordByOldPassword(string oldPasswordMD5, string newPasswordMD5)
        //{
        //    var data = new { oldPasswordMD5, newPasswordMD5 };
        //    return await PostAsync<GenericResult<object>>(JsonConvert.SerializeObject(data), "/v2/user/changePassword", false);
        //}

        ///// <summary>
        ///// 发起专用于修改密码的验证码请求
        ///// </summary>
        ///// <param name="phone">请求验证码的手机号</param>
        ///// <returns></returns>
        //public async Task<GenericResult<string>> SendingMessageToMobilePhoneNumberForChangingPassword(string phone, int countryCode)
        //{
        //    var data = new { phone, countryCode };
        //    return await PostAsync<GenericResult<string>>(JsonConvert.SerializeObject(data), "/v2/user/sendChangePasswordMessage");
        //}

        ///// <summary>
        ///// 未登录状态下修改密码
        ///// </summary>
        ///// <param name="phoneInfo">验证码请求的返回值</param>
        ///// <param name="password">新密码</param>
        ///// <param name="code">验证码</param>
        ///// <returns></returns>
        //public async Task<GenericResult<bool>> ChangePasswordByMessageCode(string phoneInfo, string code, string password)
        //{
        //    var data = new { phoneInfo, password, code };
        //    return await PostAsync<GenericResult<bool>>(JsonConvert.SerializeObject(data), "/v2/user/changePasswordWithMessage");
        //}

        ///// <summary>
        ///// 登录后修改用户名
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public async Task<GenericResult<bool>> ChangeUserName(string name)
        //{
        //    var data = new { name };
        //    return await PostAsync<GenericResult<bool>>(JsonConvert.SerializeObject(data), "/v2/user/changeName", false);
        //}

        ///// <summary>
        ///// 用户输入的密码通过此方法转换为MD5值
        ///// </summary>
        ///// <param name="input">用户输入</param>
        ///// <returns></returns>
        //public static string UserMd5(string input)
        //{
        //    using (var md5 = MD5.Create())
        //    {
        //        byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        //        StringBuilder sBuilder = new StringBuilder();
        //        for (int i = 0; i < bytes.Length; i++)
        //        {
        //            sBuilder.Append(bytes[i].ToString("x2"));
        //        }
        //        return sBuilder.ToString();
        //    }
        //}

    }
}