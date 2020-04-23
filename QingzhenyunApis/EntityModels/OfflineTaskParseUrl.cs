using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace QingzhenyunApis.EntityModels
{
    /// <summary>
    /// 一个离线下载链接的解析结果
    /// </summary>
    public partial class OfflineTaskParseUrl
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("info")]
        public Info Info { get; set; }
    }

    public partial class Info
    {
        [JsonProperty("textLink")]
        public Uri TextLink { get; set; }

        [JsonProperty("fileHash")]
        public string FileHash { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("identity")]
        public string Identity { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("dataList")]
        public IList<OfflineTaskParseFile> DataList { get; set; }
    }
}
