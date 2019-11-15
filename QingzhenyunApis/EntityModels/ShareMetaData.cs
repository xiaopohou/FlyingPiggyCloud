using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class ShareMetaData
    {
        /// <summary>
        /// 该分享的唯一ID
        /// </summary>
        [JsonProperty(PropertyName = "identity")]
        public string Identity { get; set; }

        /// <summary>
        /// 文件大小(字节)
        /// </summary>
        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        /// <summary>
        /// 文件mime，即文件的类型
        /// </summary>
        [JsonProperty(PropertyName = "mime")]
        public string Mime { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [JsonProperty(PropertyName = "userIdentity")]
        public long UserIdentity { get; set; }

        /// <summary>
        /// 文件创建时间
        /// </summary>
        [JsonProperty(PropertyName = "ctime")]
        public long Ctime { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 总是 0
        /// </summary>
        [JsonProperty(PropertyName = "flag")]
        public int Flag { get; set; }

        [JsonProperty(PropertyName = "passwordEnabled")]
        public bool PasswordEnabled { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "expireEnabled")]
        public bool ExpireEnabled { get; set; }

        [JsonProperty(PropertyName = "expire")]
        public long Expire { get; set; }

        [JsonProperty(PropertyName = "copyCountEnabled")]
        public bool CopyCountEnabled { get; set; }

        [JsonProperty(PropertyName = "copyCount")]
        public long CopyCount { get; set; }

        [JsonProperty(PropertyName = "copyCountLeft")]
        public long CopyCountLeft { get; set; }

        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }
    }
}
