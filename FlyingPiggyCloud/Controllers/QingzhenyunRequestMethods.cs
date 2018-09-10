using FlyingPiggyCloud.Controllers.Results;
using FlyingPiggyCloud.Controllers.Results.FileSystem;
using FlyingPiggyCloud.Controllers.Results.User;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyingPiggyCloud.Controllers
{
    public abstract class QingzhenyunRequestBase
    {
        protected RestClient client;

        protected static string Token;

        protected async Task<T> PostAsync<T>(string data, string uri)
        {
            return JsonConvert.DeserializeObject<T>(await client.PostAsync(data, uri));
        }

        protected T Get<T>(string uri)
        {
            return JsonConvert.DeserializeObject<T>(client.Get(uri));
        }

        public QingzhenyunRequestBase(string BaseUri)
        {
            client = new RestClient(BaseUri);
        }
    }

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
                {"password", ConverterToolKits.UserMd5(Password) },
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
                { "password", ConverterToolKits.UserMd5(Password) }
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
        public async Task<Results.ResponseMessageBase> ChangePasswordByOldPassword(string OldPassword, string NewPassword)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "OldPassword", ConverterToolKits.UserMd5(OldPassword) },
                {"newPassword", ConverterToolKits.UserMd5(NewPassword) },
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
                {"newPassword", ConverterToolKits.UserMd5(NewPassword) },
                {"code",Code },
            };
            var x = await PostAsync<ChangePasswordResponesResult>(JsonConvert.SerializeObject(data), "v1/user/changePasswordByMessage2");
            Token = x.Token;
            return x;
        }
    }

    public class FileSystemMethods : QingzhenyunRequestBase
    {
        public FileSystemMethods(string BaseUri) : base(BaseUri)
        {

        }

        /// <summary>
        /// 请求的时候，如果 name 和 path均为空，则会返回根目录
        /// </summary>
        /// <param name="Parent">该文件夹的ID</param>
        /// <param name="Path">路径</param>
        /// <param name="Page">第几页</param>
        /// <param name="PageSize">列表大小，最大值999</param>
        /// <param name="OrderBy">排序：0按文件名，1按时间</param>
        /// <param name="Type">文件类型：0显示文件，1显示文件夹，-1显示文件和文件夹(默认)</param>
        /// <returns></returns>
        public async Task<PageResponseResult> GetDirectory(string Parent="", string Path = "", int? Page = null, int? PageSize = null , int? OrderBy = null, int? Type = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if(Parent!="")
            {
                data.Add("parent", Parent);
            }
            if (Path != "")
            {
                data.Add("path", Path);
            }
            if (Page != null)
            {
                data.Add("page", Page);
            }
            if (PageSize != null)
            {
                data.Add("pageSize", PageSize);
            }
            if (OrderBy != null)
            {
                data.Add("orderBy", OrderBy);
            }
            if (Type != null)
            {
                data.Add("type", Type);
            }
            if(Token==null)
            {
                var GetToken = new Views.LoginWindow();
                GetToken.ShowDialog();
            }
            data.Add("token", Token);
            var x = await PostAsync<PageResponseResult>(JsonConvert.SerializeObject(data), "v1/files/page");
            Token = x.Token;
            return x;
        }

        /// <summary>
        /// 新建文件夹，Parent和Path参数不可共存
        /// </summary>
        /// <param name="Name">新文件夹的名字，如果其他参数为空则在根目录创建此文件夹</param>
        /// <param name="Parent">在UUID为此的文件夹内创建新文件夹</param>
        /// <param name="Path">在此路径下创建新文件夹，如果Name参数为空则创建此路径</param>
        /// <returns></returns>
        public async Task <GetMetaDataResponseResult> CreatDirectory(string Name="", string Parent="", string Path="")
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            if(Name!="")
            {
                data.Add("name", Name);
            }
            if(Parent!="")
            {
                data.Add("parent", Parent);
            }
            else if(Path!="")
            {
                data.Add("path", Path);
            }
            if (Token == null)
            {
                var GetToken = new Views.LoginWindow();
                GetToken.ShowDialog();
            }
            data.Add("token", Token);
            var x = await PostAsync<GetMetaDataResponseResult>(JsonConvert.SerializeObject(data), "v1/files/createDirectory");
            Token = x.Token;
            return x;
        }
    }
}