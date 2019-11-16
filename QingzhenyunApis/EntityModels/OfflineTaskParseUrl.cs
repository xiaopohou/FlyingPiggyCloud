using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    /// <summary>
    /// 一个离线下载链接的解析结果
    /// </summary>
    public class OfflineTaskParseUrl
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
}
