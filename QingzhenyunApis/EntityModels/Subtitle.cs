using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class Subtitle : EntityBodyBase
    {
        [JsonProperty(PropertyName = "identity")]
        public string Identity { get; set; }

        [JsonProperty(PropertyName = "hash")]
        public string Hash { get; set; }

        [JsonProperty(PropertyName = "type")]
        public int Type { get; set; }

        [JsonProperty(PropertyName = "index")]
        public int Index { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }

        [JsonProperty(PropertyName = "handlerName")]
        public string HandlerName { get; set; }

        [JsonProperty(PropertyName = "codecName")]
        public string CodecName { get; set; }

        [JsonProperty(PropertyName = "createTime")]
        public long CreateTime { get; set; }

        [JsonProperty(PropertyName = "updateTime")]
        public long UpdateTime { get; set; }

        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "downloadAddress")]
        public string DownloadAddress { get; set; }
    }
}
