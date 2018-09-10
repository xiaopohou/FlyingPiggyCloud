using FlyingPiggyCloud.Controllers.Results.User;
using System.Threading.Tasks;

namespace FlyingPiggyCloud.Controllers
{
    /// <summary>
    /// 注册流程，用于派生注册的ViewModel
    /// </summary>
    public class RegisterProgress
    {
        private string PhoneInfo { get; set; }

        public virtual string FriendlyErrorMessage { get; set; }

        private UserCenterMethods UserCenterMethods;

        protected UserInformation UserInformation { get; set; }

        /// <summary>
        /// 发起验证码请求
        /// </summary>
        /// <param name="PhoneNumber">手机号</param>
        /// <returns>如果为true，则请求信息保存于PhoneInfo变量；如果为false，则错误信息保存于FriendlyErrorMessage变量</returns>
        protected async Task<bool> GetCodeMessage(string PhoneNumber)
        {
            var Result = await UserCenterMethods.SendingMessageToMobilePhoneNumber(PhoneNumber);
            if (Result.Success == true)
            {
                PhoneInfo = Result.Result;
                return true;
            }
            else if(Result.Code!=null)
            {
                switch (Result.Code)
                {
                    case "{PHONE}_REQUIRED":
                        FriendlyErrorMessage = "提交数据缺失phone字段";
                        return false;
                    case "USER_PHONE_EXIST":
                        FriendlyErrorMessage = "此电话号码已经被注册，请直接登录";
                        return false;
                    case "SEND_MESSAGE_ERROR":
                        FriendlyErrorMessage = "系统错误，请稍后再试";
                        return false;
                    case "PHONE_NOT_VALIDATE":
                        FriendlyErrorMessage = "电话号码有误";
                        return false;
                    default:
                        FriendlyErrorMessage = string.Format("文档未定义的错误信息，Code:{0}，Message:{1}",Result.Code,Result.Message);
                        return false;
                }
            }
            else
            {
                FriendlyErrorMessage = "远程主机请求失败，且未能返回可识别的错误信息";
                return false;
            }
        }

        /// <summary>
        /// 发送注册信息
        /// </summary>
        /// <param name="Name">用户名</param>
        /// <param name="Password">密码，无需转码</param>
        /// <param name="Code">手机验证码</param>
        /// <returns>如果为true，则注册信息保存于UserInformation变量；如果为false，则错误信息保存于FriendlyErrorMessage变量</returns>
        protected async Task<bool> Register(string Name,string Password,string Code)
        {
            var Result = await UserCenterMethods.Register(Name, Password, Code, PhoneInfo);
            if (Result.Success == true)
            {
                UserInformation = Result.Result;
                return true;
            }
            else if (Result.Code != null)
            {
                switch (Result.Code)
                {
                    //case "PHONE_NOT_VALIDATE":
                    //    FriendlyErrorMessage = "电话号码有误";
                    //    return false;
                    default:
                        FriendlyErrorMessage = string.Format("文档未定义的错误信息，Code:{0}，Message:{1}", Result.Code, Result.Message);
                        return false;
                }
            }
            else
            {
                FriendlyErrorMessage = "远程主机请求失败，且未能返回可识别的错误信息";
                return false;
            }
        }

        public RegisterProgress(string BaseUri)
        {
            UserCenterMethods = new UserCenterMethods(BaseUri);
        }
    }

    /// <summary>
    /// 密码登陆流程，用于派生登陆窗格的ViewModel
    /// </summary>
    public class LoginByPasswordProgress
    {
        public UserInformation UserInformation { get; set; }

        public virtual string FriendlyErrorMessage { get; set; }

        private UserCenterMethods UserCenterMethods;

        /// <summary>
        /// 发送登陆请求
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <returns>如果为true，则注册信息保存于UserInformation变量；如果为false，则错误信息保存于FriendlyErrorMessage变量</returns>
        protected async Task<bool> Login(string UserName, string Password)
        {
            var Result = await UserCenterMethods.LoginByPassword(UserName, Password);
            if (Result.Success == true)
            {
                UserInformation = Result.Result;
                return true;
            }
            else if (Result.Code != null)
            {
                switch (Result.Code)
                {
                    //case "PHONE_NOT_VALIDATE":
                    //    FriendlyErrorMessage = "电话号码有误";
                    //    return false;
                    default:
                        FriendlyErrorMessage = string.Format("文档未定义的错误信息，Code:{0}，Message:{1}", Result.Code, Result.Message);
                        return false;
                }
            }
            else
            {
                FriendlyErrorMessage = "远程主机请求失败，且未能返回可识别的错误信息";
                return false;
            }
        }

        public LoginByPasswordProgress(string BaseUri)
        {
            UserCenterMethods = new UserCenterMethods(BaseUri);
        }
    }

    public class ForgotPasswordProgress
    {
        private string PhoneInfo { get; set; }

        public virtual string FriendlyErrorMessage { get; set; }

        private UserCenterMethods UserCenterMethods;

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <returns>如果为true，则注册信息保存于UserInformation变量；如果为false，则错误信息保存于FriendlyErrorMessage变量</returns>
        protected async Task<bool> SendMessageCode(string PhoneNumber)
        {
            var Result = await UserCenterMethods.SendingMessageToMobilePhoneNumberForChangingPassword(PhoneNumber);
            if (Result.Success == true)
            {
                PhoneInfo = Result.Result;
                return true;
            }
            else if (Result.Code != null)
            {
                switch (Result.Code)
                {
                    //case "PHONE_NOT_VALIDATE":
                    //    FriendlyErrorMessage = "电话号码有误";
                    //    return false;
                    default:
                        FriendlyErrorMessage = string.Format("文档未定义的错误信息，Code:{0}，Message:{1}", Result.Code, Result.Message);
                        return false;
                }
            }
            else
            {
                FriendlyErrorMessage = "远程主机请求失败，且未能返回可识别的错误信息";
                return false;
            }
        }

        /// <summary>
        /// 使用短信验证码修改密码
        /// </summary>
        /// <param name="NewPassword"></param>
        /// <param name="Code"></param>
        /// <returns></returns>
        protected async Task<bool> ChangePassword(string NewPassword, string Code)
        {
            var Result = await UserCenterMethods.ChangePasswordByMessageCode(PhoneInfo,Code,NewPassword);
            if (Result.Success == true)
            {
                return Result.Result;
            }
            else if (Result.Code != null)
            {
                switch (Result.Code)
                {
                    //case "PHONE_NOT_VALIDATE":
                    //    FriendlyErrorMessage = "电话号码有误";
                    //    return false;
                    default:
                        FriendlyErrorMessage = string.Format("文档未定义的错误信息，Code:{0}，Message:{1}", Result.Code, Result.Message);
                        return false;
                }
            }
            else
            {
                FriendlyErrorMessage = "远程主机请求失败，且未能返回可识别的错误信息";
                return false;
            }
        }

        public ForgotPasswordProgress(string BaseUri)
        {
            UserCenterMethods = new UserCenterMethods(BaseUri);
        }

    }
}
