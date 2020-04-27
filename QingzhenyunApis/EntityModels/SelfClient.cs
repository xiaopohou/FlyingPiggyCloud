using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class SelfClient : EntityBodyBase
    {
        [JsonProperty(PropertyName = "identity")]
        public int Identity { get; set; }

        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }

        [JsonProperty(PropertyName = "iat")]
        public int Iat { get; set; }

        [JsonProperty(PropertyName = "exp")]
        public int Exp { get; set; }

        [JsonProperty(PropertyName = "ignoreCase")]
        public bool IgnoreCase { get; set; }

        [JsonProperty(PropertyName = "ssid")]
        public string SSID { get; set; }

        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

    }

}
