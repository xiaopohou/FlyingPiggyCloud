using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class OnlineClient
    {
        [JsonProperty(PropertyName = "device")]
        public string Device { get; set; }

        //[JsonProperty(PropertyName = "ignoreCase")]
        //public bool IgnoreCase { get; set; }

        [JsonProperty(PropertyName = "loginTime")]
        public long LoginTime { get; set; }

        [JsonProperty(PropertyName = "refreshTime")]
        public long RefreshTime { get; set; }

        [JsonProperty(PropertyName = "ssid")]
        public string SSID { get; set; }

        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }
    }

}
