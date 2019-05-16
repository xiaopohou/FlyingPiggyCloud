using System;
using Newtonsoft.Json;

namespace SixCloud.Models
{
    /// <summary>
    /// 一个离线下载链接的解析结果
    /// </summary>
    internal class OfflineTaskParseUrl
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "identity")]
        public string Identity { get; set; }

        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        [JsonProperty(PropertyName = "files")]
        public OfflineTaskParseFile[] Files { get; set; }
    }

    internal class OfflineTaskAdd
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
    }

    internal class OfflineTaskParameters
    {
        public OfflineTaskParameters(string identity, string[] iginreFiles=null)
        {
            Identity = identity;
            IginreFiles = iginreFiles;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "identity")]
        public string Identity { get; set; }

        /// <summary>
        /// 所在文件的 PathIdentity
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "iginreFiles")]
        public string[] IginreFiles { get; set; }
    }
}
