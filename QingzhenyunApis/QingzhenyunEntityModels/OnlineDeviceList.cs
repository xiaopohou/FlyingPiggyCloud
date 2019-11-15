using Newtonsoft.Json;

namespace QingzhenyunApis.QingzhenyunEntityModels
{
    public class OnlineDeviceList
    {
        [JsonProperty(PropertyName = "self")]
        public SelfClient Self { get; set; }

        [JsonProperty(PropertyName = "online")]
        public OnlineClient[] Online { get; set; }
    }

}
