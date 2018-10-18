using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlyingPiggyCloud.Controllers.Results
{
    /// <summary>
    /// 所有返回消息体的基类
    /// </summary>
    public class ResponseMessageBase
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }

    /// <summary>
    /// 泛型返回体
    /// </summary>
    public class ResponesResult<T> : ResponseMessageBase
    {
        [JsonProperty(PropertyName = "result")]
        public T Result { get; set; }
    }

    namespace User
    {

        /// <summary>
        /// 短信验证码请求的返回体，Result的内容应作为第二步请求的PhoneInfo字段发送
        /// </summary>
        public class SendRegisterMessageResult: ResponseMessageBase
        {
            [JsonProperty(PropertyName = "result")]
            public string Result { get; set; }
        }

        /// <summary>
        /// 用户注册（第二步）请求的返回体
        /// </summary>
        public class RegisterResponseResult : ResponseMessageBase
        {
            [JsonProperty(PropertyName = "result")]
            public UserInformation Result { get; set; }
        }

        /// <summary>
        /// 注册或登陆成功后返回的用户信息
        /// </summary>
        public class UserInformation
        {
            [JsonProperty(PropertyName = "uuid")]
            public int UUID { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "email")]
            public string Email { get; set; }

            [JsonProperty(PropertyName = "countryCode")]
            public string CountryCode { get; set; }

            [JsonProperty(PropertyName = "phone")]
            public string Phone { get; set; }

            [JsonProperty(PropertyName = "createTime")]
            public long CreateTime { get; set; }

            [JsonProperty(PropertyName = "createIp")]
            public string CreateIp { get; set; }

            //[JsonProperty(PropertyName = "ssid")]
            //public object SSID { get; set; }

            [JsonProperty(PropertyName = "icon")]
            public string Icon { get; set; }

            [JsonProperty(PropertyName = "spaceUsed")]
            public long SpaceUsed { get; set; }

            [JsonProperty(PropertyName = "spaceCapacity")]
            public long SpaceCapacity { get; set; }

            [JsonProperty(PropertyName = "type")]
            public int Type { get; set; }

            [JsonProperty(PropertyName = "status")]
            public int Status { get; set; }
        }

        /// <summary>
        /// 登录请求的返回体
        /// </summary>
        public class LoginResponseResult:ResponseMessageBase
        {
            [JsonProperty(PropertyName = "result")]
            public UserInformation Result { get; set; }
        }

        /// <summary>
        /// 修改密码请求的返回体
        /// </summary>
        public class ChangePasswordResponesResult:ResponseMessageBase
        {
            [JsonProperty(PropertyName = "result")]
            public bool Result { get; set; }
        }
    }

    namespace FileSystem
    {
        /// <summary>
        /// 文件或文件夹的基本信息
        /// </summary>
        public class FileMetaData
        {
            /// <summary>
            /// 该文件/文件夹的唯一ID
            /// </summary>
            [JsonProperty(PropertyName = "uuid")]
            public string UUID { get; set; }

            /// <summary>
            /// 文件名
            /// </summary>
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            /// <summary>
            /// 文件mime，即文件的类型
            /// </summary>
            [JsonProperty(PropertyName = "mime")]
            public string Mime { get; set; }

            /// <summary>
            /// 0:为文件,1:目录
            /// </summary>
            [JsonProperty(PropertyName = "type")]
            public int Type { get; set; }

            /// <summary>
            /// 父目录id
            /// </summary>
            [JsonProperty(PropertyName = "parent")]
            public string Parent { get; set; }

            /// <summary>
            /// 文件创建时间
            /// </summary>
            [JsonProperty(PropertyName = "ctime")]
            public long Ctime { get; set; }

            /// <summary>
            /// 文件修改时间
            /// </summary>
            [JsonProperty(PropertyName = "mtime")]
            public long Mtime { get; set; }

            /// <summary>
            /// 文件访问时间
            /// </summary>
            [JsonProperty(PropertyName = "atime")]
            public long Atime { get; set; }

            /// <summary>
            /// 用户id
            /// </summary>
            [JsonProperty(PropertyName = "userId")]
            public long UserId { get; set; }


            /// <summary>
            /// 路径
            /// </summary>
            [JsonProperty(PropertyName = "path")]
            public string Path { get; set; }

            /// <summary>
            /// 该文件或文件夹的访问路径
            /// </summary>
            [JsonProperty(PropertyName = "ext")]
            public string Ext { get; set; }

            /// <summary>
            /// 文件大小(字节)
            /// </summary>
            [JsonProperty(PropertyName = "size")]
            public long Size { get; set; }

            /// <summary>
            /// 总是 0
            /// </summary>
            [JsonProperty(PropertyName = "flag")]
            public int Flag { get; set; }

            /// <summary>
            /// 预览状态
            /// </summary>
            [JsonProperty(PropertyName = "preview")]
            public int Preview { get; set; }

            /// <summary>
            /// 0:正常文件，1:回收站
            /// </summary>
            [JsonProperty(PropertyName = "recycle")]
            public int Recycle { get; set; }

            /// <summary>
            /// 文件版本
            /// </summary>
            [JsonProperty(PropertyName = "version")]
            public int Version { get; set; }

            /// <summary>
            /// 是否锁定
            /// </summary>
            [JsonProperty(PropertyName = "locking")]
            public bool Locking { get; set; }

            /// <summary>
            /// 如果锁定,文件锁定的情况下，当前后台正进行的操作
            /// </summary>
            [JsonProperty(PropertyName = "opt")]
            public int Opt { get; set; }

            /// <summary>
            /// 文件下载地址
            /// </summary>
            [JsonProperty(PropertyName = "downloadAddress")]
            public string DownloadAddress { get; set; }
        }

        /// <summary>
        /// 文件列表页
        /// </summary>
        public class FilesListPage
        {
            /// <summary>
            /// 当前页码
            /// </summary>
            [JsonProperty(PropertyName = "page")]
            public int Page { get; set; }

            /// <summary>
            /// 每页展示的文件数，Max：999
            /// </summary>
            [JsonProperty(PropertyName = "pageSize")]
            public int PageSize { get; set; }

            /// <summary>
            /// 文件总数
            /// </summary>
            [JsonProperty(PropertyName = "totalCount")]
            public int TotalCount { get; set; }

            /// <summary>
            /// 页总数
            /// </summary>
            [JsonProperty(PropertyName = "totalPage")]
            public int TotalPage { get; set; }

            /// <summary>
            /// 本页目录及文件列表
            /// </summary>
            [JsonProperty(PropertyName = "list")]
            public FileMetaData[] List { get; set; }
            
            /// <summary>
            /// 当前目录MetaData
            /// </summary>
            [JsonProperty(PropertyName = "info")]
            public FileMetaData DictionaryInformation { get; set; }
        }

        /// <summary>
        /// 指定目录下文件列表页请求的返回体
        /// </summary>
        public class PageResponseResult:ResponseMessageBase
        {
            [JsonProperty(PropertyName = "result")]
            public FilesListPage Result { get; set; }
        }

        //封装接口缺失：列出指定目录下的文件夹（仅列表）

        /// <summary>
        /// 通过uuid或path获取文件/文件夹MetaData请求的返回体
        /// 新建文件夹请求亦使用此返回体
        /// </summary>
        public class GetMetaDataResponseResult:ResponseMessageBase
        {
            [JsonProperty(PropertyName = "result")]
            public FileMetaData Result { get; set; }
        }

        /// <summary>
        /// 开启上传请求的返回体
        /// 这个返回体可能不一定正确工作
        /// </summary>
        public class UploadResponseResult:ResponseMessageBase
        {
            [JsonProperty(PropertyName = "result")]
            public Dictionary<string,string> Result { get; set; }
        }


    }

    namespace OfflineDownload
    {

    }
}