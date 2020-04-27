using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class DestinationInformation : EntityBodyBase
    {
        [JsonProperty(PropertyName = "destination")]
        public string Destination { get; set; }

        [JsonProperty(PropertyName = "expireTime")]
        public long ExpireTime { get; set; }
    }
}
