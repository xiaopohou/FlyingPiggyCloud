using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class Label
    {
        [JsonProperty("identity")]
        public int Identity { get; set; }

        [JsonProperty("userIdentity")]
        public long UserIdentity { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("createTime")]
        public long CreateTime { get; set; }
    }
}
