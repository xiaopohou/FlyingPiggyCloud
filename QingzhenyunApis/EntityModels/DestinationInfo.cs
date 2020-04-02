using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class DestinationInfo
    {
        [JsonProperty(PropertyName = "destination")]
        public string Destination { get; set; }

        [JsonProperty(PropertyName = "expireTime")]
        public long ExpireTime { get; set; }
    }
}
